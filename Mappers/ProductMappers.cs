using SmartFridgeAPI.Dtos.Product;
using SmartFridgeAPI.Models;

namespace SmartFridgeAPI.Mappers
{
    public static class ProductMappers
    {
        public static ProductDto ToProductDto(this Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                UrgencyName = product.Urgency?.Name ?? "Unknown"
            };
        }

        public static Product ToProductFromCreateDto(this CreateProductRequestDto product)
        {
            return new Product
            {
                Name = product.Name!,
                UrgencyId = product.UrgencyId
            };
        }
    }
}