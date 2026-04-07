using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SupermarketShopListAPI.Dtos.Urgency;
using SupermarketShopListAPI.Models;

namespace SupermarketShopListAPI.Mappers
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