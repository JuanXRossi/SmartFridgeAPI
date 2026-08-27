using System.ComponentModel.DataAnnotations;

namespace SmartFridgeAPI.Dtos.Account
{
    public class UpdateDto
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [MinLength(4, ErrorMessage = "El nombre de usuario tiene que tener al menos 4 caracteres")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress]
        public string? Email { get; set; }

        public string? Password { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MinLength(3, ErrorMessage = "El nombre tiene que tener al menos 3 caracteres")]
        [RegularExpression(@"^[A-Z][a-z]+(\s[A-Z][a-z]+)*$", ErrorMessage = "El nombre debe comenzar con mayúscula, seguido de dos o más minúsculas y contener solo letras")]
        public string? Name { get; set; }
    }
}