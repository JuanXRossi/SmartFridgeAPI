using Microsoft.AspNetCore.Identity;
using SmartFridgeAPI.Dtos.Account;
using SmartFridgeAPI.Interfaces;
using SmartFridgeAPI.Models;
using SmartFridgeAPI.Models.Data;

namespace SmartFridgeAPI.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<User> _userManager;

        public AccountRepository(ApplicationDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
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