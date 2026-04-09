using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartFridgeAPI.Controllers.Helpers
{
    public class QueryObject
    {
        public string? ProductName { get; set; } = null;
        
        public string? UrgencyName { get; set; } = null;

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}