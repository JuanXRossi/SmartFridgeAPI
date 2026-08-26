using SmartFridgeAPI.Dtos.Account;
using SmartFridgeAPI.Models;

namespace SmartFridgeAPI.Interfaces
{
    public interface IAccountRepository
    {
        Task<UserTransactionResult> RegisterUserAsync(User user, RegisterDto registerDto);
        Task<UserTransactionResult> UpdateUserAsync(User user, UpdateDto updateDto);
        Task<UserTransactionResult> SendConfirmationEmailAsync(User user);
        Task<UserTransactionResult> ConfirmEmailAsync(string userId, string token);
        Task SendPasswordResetEmailAsync(User user); // always "succeeds" from caller's perspective
        Task<UserTransactionResult> ResetPasswordAsync(string userId, string token, string newPassword);
    }
}