using System.ComponentModel.DataAnnotations;

namespace SmartFridgeAPI.Dtos.Account
{
    public class UpdateDto
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress]
        public string? Email { get; set; }

        public string? Password { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [RegularExpression(@"^[A-Z][a-z]+(\s[A-Z][a-z]+)*$")]
        public string? Name { get; set; }
    }
}