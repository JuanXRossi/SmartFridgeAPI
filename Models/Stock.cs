using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupermarketShopListAPI.Models
{
    public class Stock
    {
        public int Id { get; set; }

        public List<Product>? Products { get; set; }

        public int Amount { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}