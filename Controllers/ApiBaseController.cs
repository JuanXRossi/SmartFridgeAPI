using Microsoft.AspNetCore.Mvc;

namespace SmartFridgeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class ApiBaseController<T> : ControllerBase
    {
        protected readonly ILogger<T> _logger;
        protected ApiBaseController(ILogger<T> logger)
        {
            _logger = logger;
        }
    }
}