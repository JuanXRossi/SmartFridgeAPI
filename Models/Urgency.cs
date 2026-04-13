namespace SmartFridgeAPI.Models
{
    public class Urgency
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int MinAmount { get; set; }
    }
}