using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupermarketShopListAPI.Dtos.Product
{
    public class UpdateProductRequestDto
    {
        public string Name { get; set; } = string.Empty;

        public int UrgencyId { get; set; }
    }
}