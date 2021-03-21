using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Microsoft.UnifiedRedisPlatform.Service.API.Controllers
{
    [Route("api/probe")]
    [ApiController]
    public class ProbeController: ControllerBase
    {
        [Route("ping")]
        public IActionResult Ping()
        {
            return new OkObjectResult("Pong");
        }

        [Authorize]
        [Route("pingauth")]
        public IActionResult PingAuth()
        {
            var user = HttpContext.User;
            var claims = string.Join(';', user.Claims.Select(claim => $"{claim.Type}-{claim.Value}"));

            return new OkObjectResult($"Pong with claims {claims}");
        }
    }
}
