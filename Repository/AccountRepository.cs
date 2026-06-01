using Microsoft.AspNetCore.Identity;
using SmartFridgeAPI.Dtos.Account;
using SmartFridgeAPI.Interfaces;
using SmartFridgeAPI.Models;
using SmartFridgeAPI.Models.Data;

namespace SmartFridgeAPI.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private ApplicationDBContext _context;
        private UserManager<User> _userManager;

        public AccountRepository(ApplicationDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<UpdateUserResult> UpdateUserAsync(User user, UpdateDto updateDto)
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
                return UpdateUserResult.Failure(message);
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, updateDto.Password);

                if (!passwordResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    var message = string.Join(", ", passwordResult.Errors.Select(e => e.Description));
                    return UpdateUserResult.Failure(message);
                }
            }

            await transaction.CommitAsync();
            return UpdateUserResult.Success();
        }
    }
}