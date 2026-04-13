using SmartFridgeAPI.Models;

namespace SmartFridgeAPI.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);

        string CreateRefreshToken();
    }
}