using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Microsoft.UnifiedRedisPlatform.Manager.API.Controllers
{
    [ApiController]
    [Route("api/probe")]
    public class ProbeController: ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ProbeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("ping")]
        public IActionResult Ping([FromQuery] string configKey = null)
        {
            if (string.IsNullOrWhiteSpace(configKey))
                return new OkObjectResult("Pong");
            return new OkObjectResult(_configuration[configKey]);
        }
    }
}
