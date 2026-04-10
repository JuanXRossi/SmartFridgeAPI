using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartFridgeAPI.Dtos.Account
{
    public class RegisterDto
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z][a-z]+(\s[A-Z][a-z]+)*$")]
        public string? Name { get; set; }
    }
}