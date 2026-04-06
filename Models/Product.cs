using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SupermarketShopListAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int UrgencyId { get; set; }

        public Urgency? Urgency { get; set; }
        [JsonIgnore]
        public List<Stock>? Stocks { get; set; }
    }
}