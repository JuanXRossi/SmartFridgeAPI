using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartFridgeAPI.Dtos.Product;
using SmartFridgeAPI.Interfaces;
using SmartFridgeAPI.Models;
using SmartFridgeAPI.Models.Data;

namespace SmartFridgeAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDBContext _context;
        public ProductRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<bool> AnyWithUrgencyAsync(int urgencyId)
        {
            return await _context.Products.AnyAsync(p => p.UrgencyId == urgencyId);
        }

        public async Task<Product> CreateAsync(Product productModel)
        {
            await _context.Products.AddAsync(productModel);
            await _context.SaveChangesAsync();

            return productModel;
        }

        public async Task<Product?> DeleteAsync(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return null;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.Include(p => p.Urgency).ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.Include(p => p.Urgency).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product?> GetByModelAsync(Product productModel)
        {
            return await _context.Products.Include(p => p.Urgency).FirstOrDefaultAsync(p => p.Id == productModel.Id);
        }

        public async Task<Product?> UpdateAsync(int id, UpdateProductRequestDto productDto)
        {
            var productModel = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (productModel == null)
            {
                return null;
            }

            productModel.Name = productDto.Name!;
            productModel.UrgencyId = productDto.UrgencyId;

            await _context.SaveChangesAsync();

            return productModel;
        }
    }
}