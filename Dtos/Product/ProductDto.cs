namespace SmartFridgeAPI.Dtos.Product
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ProductUrgency Urgency { get; set; } = new();
    }

    public class ProductUrgency
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}