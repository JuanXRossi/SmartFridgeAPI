using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartFridgeAPI.Models;

namespace SmartFridgeAPI.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);

        string CreateRefreshToken();
    }
}