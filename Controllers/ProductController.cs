using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    }
}