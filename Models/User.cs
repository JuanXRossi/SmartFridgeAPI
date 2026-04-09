using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SmartFridgeAPI.Models.Enums;

namespace SmartFridgeAPI.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; } = string.Empty;

        public Role Role { get; set; }
    }
}