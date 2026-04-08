using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartFridgeAPI.Dtos.Urgency;
using SmartFridgeAPI.Models;

namespace SmartFridgeAPI.Mappers
{
    public static class UrgencyMappers
    {
        public static Urgency ToUrgencyFromCreateDto(this CreateUrgencyRequestDto urgency)
        {
            return new Urgency
            {
                Name = urgency.Name,
                MinAmount = urgency.MinAmount
            };
        }
    }
}