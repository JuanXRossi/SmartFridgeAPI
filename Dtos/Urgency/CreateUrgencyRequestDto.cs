using System.ComponentModel.DataAnnotations;

namespace SmartFridgeAPI.Dtos.Urgency
{
    public class CreateUrgencyRequestDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MinLength(4, ErrorMessage = "El nombre tiene que tener al menos 4 caracteres")]
        [MaxLength(20, ErrorMessage = "El nombre tiene que tener a lo sumo 20 caracteres")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "La mínima cantidad es obligatoria")]
        [Range(1, 10, ErrorMessage = "La mínima cantidad tiene que ser un valor entre 0 y 4")]
        public int MinAmount { get; set; }
    }
}