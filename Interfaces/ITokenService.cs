using SmartFridgeAPI.Models;

namespace SmartFridgeAPI.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user, IList<string> roles);

        string CreateRefreshToken();
    }
}