using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartFridgeAPI.Dtos.Account
{
    public class UserPublicInfoDto
    {
        public string UserName { get; set; }
        
        public string Name { get; set; }

        public string Email { get; set; }

        public List<string> Roles { get; set; }
    }
}