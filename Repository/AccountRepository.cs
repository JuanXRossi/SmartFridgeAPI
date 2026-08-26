using Microsoft.AspNetCore.Identity;
using SmartFridgeAPI.Dtos.Account;
using SmartFridgeAPI.Interfaces;
using SmartFridgeAPI.Models;
using SmartFridgeAPI.Data;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace SmartFridgeAPI.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;

        public AccountRepository(ApplicationDBContext context, UserManager<User> userManager, IEmailService emailService, IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
            _config = config;
        }

        public async Task<UserTransactionResult> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return UserTransactionResult.Failure("Usuario no encontrado.");

            if (user.EmailConfirmed)
                return UserTransactionResult.Success();

            string decodedToken;
            try
            {
                decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            }
            catch
            {
                return UserTransactionResult.Failure("Token inválido.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (!result.Succeeded)
            {
                var message = string.Join(", ", result.Errors.Select(e => e.Description));
                return UserTransactionResult.Failure(message);
            }

            return UserTransactionResult.Success();
        }

        public async Task<UserTransactionResult> RegisterUserAsync(User user, RegisterDto registerDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var createdUser = await _userManager.CreateAsync(user, registerDto.Password!);

            if (!createdUser.Succeeded)
            {
                await transaction.RollbackAsync();
                var message = string.Join(", ", createdUser.Errors.Select(e => e.Description));
                return UserTransactionResult.Failure(message);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");

            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync();
                var message = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                return UserTransactionResult.Failure(message, isServerError: true);
            }

            await transaction.CommitAsync();
            return UserTransactionResult.Success();
        }

        public async Task<UserTransactionResult> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return UserTransactionResult.Failure("Token inválido o expirado.");

            string decodedToken;
            try
            {
                decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            }
            catch
            {
                return UserTransactionResult.Failure("Token inválido o expirado.");
            }

            var isValid = await _userManager.VerifyUserTokenAsync(user, "PasswordResetTokenProvider", "ResetPassword", decodedToken);
            
            if (!isValid) return UserTransactionResult.Failure("Token inválido o expirado.");
            
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, newPassword);

            if (!result.Succeeded)
            {
                var message = string.Join(", ", result.Errors.Select(e => e.Description));
                return UserTransactionResult.Failure(message);
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);

            return UserTransactionResult.Success();
        }

        public async Task<UserTransactionResult> SendConfirmationEmailAsync(User user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var link = $"{_config["Frontend:BaseUrl"]}/confirm-email" +
                    $"?userId={user.Id}&token={encodedToken}";

            try
            {
                await _emailService.SendEmailConfirmationAsync(user.Email!, user.Name, link);
                return UserTransactionResult.Success();
            }
            catch
            {
                return UserTransactionResult.Failure("No se pudo enviar el correo de confirmación. Podés reenviarlo más tarde.", isServerError: true);
            }
        }

        public async Task SendPasswordResetEmailAsync(User user)
        {
            var token = await _userManager.GenerateUserTokenAsync(user, "PasswordResetTokenProvider", "ResetPassword");
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var link = $"{_config["Frontend:BaseUrl"]}/reset-password" +
                    $"?userId={user.Id}&token={encodedToken}";

            try
            {
                await _emailService.SendPasswordResetAsync(user.Email!, user.Name, link);
            }
            catch
            {}
        }

        public async Task<UserTransactionResult> UpdateUserAsync(User user, UpdateDto updateDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            user.UserName = updateDto.Username;
            user.Email = updateDto.Email;
            user.Name = updateDto.Name;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                await transaction.RollbackAsync();
                var message = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                return UserTransactionResult.Failure(message);
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, updateDto.Password);

                if (!passwordResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    var message = string.Join(", ", passwordResult.Errors.Select(e => e.Description));
                    return UserTransactionResult.Failure(message);
                }
            }

            await transaction.CommitAsync();
            return UserTransactionResult.Success();
        }
    }
}