using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartFridgeAPI.Dtos.Urgency;
using SmartFridgeAPI.Interfaces;
using SmartFridgeAPI.Mappers;

namespace SmartFridgeAPI.Controllers
{
    [Authorize]
    public class UrgencyController : ApiBaseController<UrgencyController>
    {
        private readonly IUrgencyRepository _urgencyRepository;
        private readonly IProductRepository _productRepository;

        public UrgencyController(
            IUrgencyRepository urgencyRepository, 
            IProductRepository productRepository, 
            ILogger<UrgencyController> logger)
        : base(logger)
        {
            _urgencyRepository = urgencyRepository;
            _productRepository = productRepository;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var urgencies = await _urgencyRepository.GetAllAsync();

            return Ok(urgencies);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var urgency = await _urgencyRepository.GetByIdAsync(id);

            if (urgency == null)
            {
                return NotFound();
            }

            return Ok(urgency);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUrgencyRequestDto urgencyDto)
        {
            var urgencyModel = urgencyDto.ToUrgencyFromCreateDto();

            await _urgencyRepository.CreateAsync(urgencyModel);

            return CreatedAtAction(nameof(GetById), new { id = urgencyModel.Id }, urgencyModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUrgencyRequestDto urgencyDto)
        {
            var urgencyModel = await _urgencyRepository.UpdateAsync(id, urgencyDto);

            if (urgencyModel == null)
            {
                return NotFound();
            }

            return Ok(urgencyModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("{id:int}")]
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