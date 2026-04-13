using System.ComponentModel.DataAnnotations;

namespace SmartFridgeAPI.Dtos.Account
{
    public class RefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}