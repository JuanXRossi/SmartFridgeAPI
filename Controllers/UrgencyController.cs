using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupermarketShopListAPI.Dtos.Urgency;
using SupermarketShopListAPI.Mappers;
using SupermarketShopListAPI.Models.Data;

namespace SupermarketShopListAPI.Controllers
{
    [Route("/api/urgency")]
    [ApiController]
    public class UrgencyController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public UrgencyController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var urgencies = await _context.Urgencies.ToListAsync();

            return Ok(urgencies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var urgency = await _context.Urgencies.FindAsync(id);

            if (urgency == null)
            {
                return NotFound();
            }

            return Ok(urgency);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUrgencyRequestDto urgencyDto)
        {
            var urgencyModel = urgencyDto.ToUrgencyFromCreateDto();

            await _context.Urgencies.AddAsync(urgencyModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = urgencyModel.Id }, urgencyModel);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUrgencyRequestDto urgencyDto)
        {
            var urgencyModel = await _context.Urgencies.FirstOrDefaultAsync(u => u.Id == id);

            if (urgencyModel == null)
            {
                return NotFound();
            }

            urgencyModel.Name = urgencyDto.Name;
            urgencyModel.MinAmount = urgencyDto.MinAmount;

            await _context.SaveChangesAsync();

            return Ok(urgencyModel);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var urgency = await _context.Urgencies.FirstOrDefaultAsync(u => u.Id == id);

            if (urgency == null)
            {
                return NotFound();
            }

            _context.Urgencies.Remove(urgency);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}