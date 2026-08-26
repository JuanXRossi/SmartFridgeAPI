namespace SmartFridgeAPI.Dtos.Urgency
{
    public class UrgencyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MinAmount { get; set; }
    }
}