using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartFridgeAPI.Dtos.Urgency;
using SmartFridgeAPI.Interfaces;
using SmartFridgeAPI.Mappers;
using SmartFridgeAPI.Models.Data;

namespace SmartFridgeAPI.Controllers
{
    [Route("/api/urgency")]
    [ApiController]
    public class UrgencyController : ControllerBase
    {
        private readonly IUrgencyRepository _urgencyRepository;
        private readonly IProductRepository _productRepository;

        public UrgencyController(IUrgencyRepository urgencyRepository, IProductRepository productRepository)
        {
            _urgencyRepository = urgencyRepository;
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var urgencies = await _urgencyRepository.GetAllAsync();

            return Ok(urgencies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var urgency = await _urgencyRepository.GetByIdAsync(id);

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

            await _urgencyRepository.CreateAsync(urgencyModel);

            return CreatedAtAction(nameof(GetById), new { id = urgencyModel.Id }, urgencyModel);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUrgencyRequestDto urgencyDto)
        {
            var urgencyModel = await _urgencyRepository.UpdateAsync(id, urgencyDto);

            if (urgencyModel == null)
            {
                return NotFound();
            }

            return Ok(urgencyModel);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var inUse = await _productRepository.AnyWithUrgencyAsync(id);

            if (inUse)
            {
                return BadRequest($"Urgencia con id {id} está en uso por uno o más productos.");
            }

            var urgency = await _urgencyRepository.DeleteAsync(id);

            if (urgency == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}