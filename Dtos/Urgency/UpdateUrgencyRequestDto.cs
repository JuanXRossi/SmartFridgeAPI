using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartFridgeAPI.Dtos.Urgency
{
    public class UpdateUrgencyRequestDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MinLength(5, ErrorMessage = "El nombre tiene que tener al menos 5 caracteres")]
        [MaxLength(20, ErrorMessage = "El nombre tiene que tener a lo sumo 20 caracteres")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "La mínima cantidad es obligatoria")]
        [Range(0, 4, ErrorMessage = "La mínima cantidad tiene que ser un valor entre 0 y 4")]
        public int MinAmount { get; set; }
    }
}