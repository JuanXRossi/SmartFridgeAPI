using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartFridgeAPI.Controllers.Helpers;
using SmartFridgeAPI.Dtos.Product;
using SmartFridgeAPI.Models;

namespace SmartFridgeAPI.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync(QueryObject query);

        Task<Product?> GetByIdAsync(int id);

        Task<Product?> GetByModelAsync(Product productModel);

        Task<Product> CreateAsync(Product productModel);

        Task<Product?> UpdateAsync(int id, UpdateProductRequestDto productDto);

        Task<Product?> DeleteAsync(int id);

        Task<bool> AnyWithUrgencyAsync(int urgencyId);
    }
}