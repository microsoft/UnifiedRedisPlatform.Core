using System.Linq;
using CQRS.Mediatr.Lite;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AppInsights.EnterpriseTelemetry.Web.Extension.Filters;
using Microsoft.UnifiedPlatform.Service.Common.Models;
using Microsoft.UnifiedPlatform.Service.Application.Commands;

namespace Microsoft.UnifiedRedisPlatform.Service.API.Controllers
{
    [Route("api/token")]
    [ServiceFilter(typeof(RequestResponseLoggerFilterAttribute))]
    [ApiController]
    public class TokenController: ControllerBase
    {
        private readonly ICommandBus _commandBus;
        public TokenController(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody]TokenRequestModel tokenRequestModel)
        {
            var secret = GetSingleValueFromHeader("X-URP-Secret");
            var authenticateClientCommand = new AuthenticateClientCommand(tokenRequestModel.ClusterName, tokenRequestModel.AppName, secret);
            var tokenResult = await _commandBus.Send(authenticateClientCommand);
            return new OkObjectResult(tokenResult);
        }

        private string GetSingleValueFromHeader(string headerKey)
        {
            if (!Request.Headers.ContainsKey(headerKey))
                return null;
            return Request.Headers[headerKey].FirstOrDefault();
        }
    }
}
