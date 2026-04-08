using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartFridgeAPI.Dtos.Product
{
    public class CreateProductRequestDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MinLength(5, ErrorMessage = "El nombre tiene que tener al menos 5 caracteres")]
        [MaxLength(70, ErrorMessage = "El nombre tiene que tener a lo sumo 70 caracteres")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "El ID de urgencia es obligatorio")]
        [Range(1, 10, ErrorMessage = "El ID de urgencia tiene que ser un valor entre 1 y 10")]
        public int UrgencyId { get; set; }
    }
}