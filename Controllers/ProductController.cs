using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartFridgeAPI.Helpers;
using SmartFridgeAPI.Dtos.Product;
using SmartFridgeAPI.Interfaces;
using SmartFridgeAPI.Mappers;
using SmartFridgeAPI.Models.Data;

namespace SmartFridgeAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IUrgencyRepository _urgencyRepository;
        public ProductController(IProductRepository productRepository, IUrgencyRepository urgencyRepository)
        {
            _productRepository = productRepository;
            _urgencyRepository = urgencyRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            var products = await _productRepository.GetAllAsync(query);
                
            var productsDto = products.Select(p => p.ToProductDto());

            return Ok(productsDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product.ToProductDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequestDto productDto)
        {
            var urgencyExists = await _urgencyRepository.ExistsAsync(productDto.UrgencyId);

            if (!urgencyExists)
            {
                return BadRequest($"Urgencia con id {productDto.UrgencyId} no existe");
            }

            var productModel = productDto.ToProductFromCreateDto();

            await _productRepository.CreateAsync(productModel);

            var product = await _productRepository.GetByModelAsync(productModel);

            if (product == null)
            {
                return StatusCode(500, "Error al recuperar producto creado");
            }

            return CreatedAtAction(nameof(GetById), new { id = productModel.Id }, product.ToProductDto());
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProductRequestDto updateDto)
        {
            var urgencyExists = await _urgencyRepository.ExistsAsync(updateDto.UrgencyId);

            if (!urgencyExists)
            {
                return BadRequest($"Urgencia con id {updateDto.UrgencyId} no existe.");
            }

            var productModel = await _productRepository.UpdateAsync(id, updateDto);

            if (productModel == null)
            {
                return NotFound();
            }

            var product = await _productRepository.GetByModelAsync(productModel);

            if (product == null)
            {
                return StatusCode(500, "Error al recuperar producto actualizado");
            }

            return Ok(product.ToProductDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var product = await _productRepository.DeleteAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}