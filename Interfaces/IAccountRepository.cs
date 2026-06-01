using SmartFridgeAPI.Dtos.Account;
using SmartFridgeAPI.Models;

namespace SmartFridgeAPI.Interfaces
{
    public interface IAccountRepository
    {
        Task<UpdateUserResult> UpdateUserAsync(User user, UpdateDto updateDto);
    }
}