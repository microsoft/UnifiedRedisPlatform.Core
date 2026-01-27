using System.Linq;
using Microsoft.CQRS;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.UnifiedPlatform.Service.Common.Models;
using Microsoft.UnifiedPlatform.Service.Application.Commands;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;

namespace Microsoft.UnifiedRedisPlatform.Service.API.Controllers
{
    [Authorize]
    [Route("api/logs")]
    public class LogsController: BaseController
    {
        private readonly ICommandBus _commandBus;

        public LogsController(ICommandBus commandBus, AppMetadataConfiguration configuration)
            :base(configuration)
        {
            _commandBus = commandBus;
        }

        [HttpPost]
        public async Task<IActionResult> Log([FromBody] List<Log> logs)
        {
            if (logs == null || !logs.Any())
                return new NotFoundObjectResult("No records were found");

            var cluster = GetClusterFromClaims();
            var app = GetAppFromClaims();

            var command = new LogClientInfoCommand(logs, cluster, app);
            var logResult = await _commandBus.Send(command);
            return new OkObjectResult(logResult);
        }
    }
}
