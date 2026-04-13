using System.Text.Json.Serialization;

namespace SmartFridgeAPI.Models
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