using SmartFridgeAPI.Dtos.Account;
using SmartFridgeAPI.Models;

namespace SmartFridgeAPI.Interfaces
{
    public interface IAccountRepository
    {
        Task<UserTransactionResult> RegisterUserAsync(User user, RegisterDto registerDto);
        Task<UserTransactionResult> UpdateUserAsync(User user, UpdateDto updateDto);
    }
}