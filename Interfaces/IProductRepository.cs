using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SupermarketShopListAPI.Dtos.Product;
using SupermarketShopListAPI.Models;

namespace SupermarketShopListAPI.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();

        Task<Product?> GetByIdAsync(int id);

        Task<Product?> GetByModelAsync(Product productModel);

        Task<Product> CreateAsync(Product productModel);

        Task<Product?> UpdateAsync(int id, UpdateProductRequestDto productDto);

        Task<Product?> DeleteAsync(int id);
    }
}