using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SmartFridgeAPI.Extensions
{
    public static class ClaimsExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(JwtRegisteredClaimNames.Name)!;
        }
    }
}