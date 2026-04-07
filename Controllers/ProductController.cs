using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupermarketShopListAPI.Dtos.Product;
using SupermarketShopListAPI.Mappers;
using SupermarketShopListAPI.Models.Data;

namespace SupermarketShopListAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public ProductController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _context.Products
                .Include(p => p.Urgency)
                .ToListAsync();
                
            var productsDto = products.Select(p => p.ToProductDto());

            return Ok(productsDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product.ToProductDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequestDto productDto)
        {
            var productModel = productDto.ToProductFromCreateDto();

            await _context.Products.AddAsync(productModel);
            await _context.SaveChangesAsync();

            var product = await _context.Products
                .Include(p => p.Urgency)
                .FirstOrDefaultAsync(p => p.Id == productModel.Id);

            if (product == null)
            {
                return StatusCode(500, "Error al recuperar producto creado");
            }

            return CreatedAtAction(nameof(GetById), new { id = productModel.Id }, product.ToProductDto());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProductRequestDto updateDto)
        {
            var productModel = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (productModel == null)
            {
                return NotFound();
            }

            productModel.Name = updateDto.Name;
            productModel.UrgencyId = updateDto.UrgencyId;

            await _context.SaveChangesAsync();

            var product = await _context.Products
                .Include(p => p.Urgency)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return StatusCode(500, "Error al recuperar producto actualizado");
            }

            return Ok(product.ToProductDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}