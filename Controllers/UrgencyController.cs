using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult GetAll()
        {
            var urgencies = _context.Urgencies.ToList();

            return Ok(urgencies);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var urgency = _context.Urgencies.Find(id);

            if (urgency == null)
            {
                return NotFound();
            }

            return Ok(urgency);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateUrgencyRequestDto urgencyDto)
        {
            var urgencyModel = urgencyDto.ToUrgencyFromCreateDto();

            _context.Urgencies.Add(urgencyModel);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = urgencyModel.Id }, urgencyModel);
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] UpdateUrgencyRequestDto urgencyDto)
        {
            var urgencyModel = _context.Urgencies.FirstOrDefault(u => u.Id == id);

            if (urgencyModel == null)
            {
                return NotFound();
            }

            urgencyModel.Name = urgencyDto.Name;
            urgencyModel.MinAmount = urgencyDto.MinAmount;

            _context.SaveChanges();

            return Ok(urgencyModel);
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var urgency = _context.Urgencies.FirstOrDefault(u => u.Id == id);

            if (urgency == null)
            {
                return NotFound();
            }

            _context.Urgencies.Remove(urgency);
            _context.SaveChanges();

            return NoContent();
        }
    }
}