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
        public IActionResult GetAll()
        {
            var products = _context.Products
                .Include(p => p.Urgency)
                .ToList()
                .Select(p => p.ToProductDto());

            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product.ToProductDto());
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateProductRequestDto productDto)
        {
            var productModel = productDto.ToProductFromCreateDto();

            _context.Products.Add(productModel);
            _context.SaveChanges();

            var product = _context.Products
                .Include(p => p.Urgency)
                .FirstOrDefault(p => p.Id == productModel.Id);

            if (product == null)
            {
                return StatusCode(500, "Error al recuperar producto creado");
            }

            return CreatedAtAction(nameof(GetById), new { id = productModel.Id }, product.ToProductDto());
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] UpdateProductRequestDto updateDto)
        {
            var productModel = _context.Products.FirstOrDefault(p => p.Id == id);

            if (productModel == null)
            {
                return NotFound();
            }

            productModel.Name = updateDto.Name;
            productModel.UrgencyId = updateDto.UrgencyId;

            _context.SaveChanges();

            var product = _context.Products
                .Include(p => p.Urgency)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return StatusCode(500, "Error al recuperar producto actualizado");
            }

            return Ok(product.ToProductDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            _context.SaveChanges();

            return NoContent();
        }
    }
}