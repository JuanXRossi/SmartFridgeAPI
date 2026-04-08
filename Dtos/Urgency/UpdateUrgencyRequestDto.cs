using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartFridgeAPI.Dtos.Urgency
{
    public class UpdateUrgencyRequestDto
    {
        public string Name { get; set; } = string.Empty;

        public int MinAmount { get; set; }
    }
}