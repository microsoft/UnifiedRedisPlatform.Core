using System.Linq;
using Microsoft.CQRS;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.UnifiedRedisPlatform.Manager.Domain.Queries;
using Microsoft.UnifiedRedisPlatform.Manager.Domain.Commands;

namespace Microsoft.UnifiedRedisPlatform.Manager.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/clusters")]
    public class ClustersController: ControllerBase
    {
        private readonly IQueryService _queryService;
        private readonly ICommandBus _commandBus;

        public ClustersController(IQueryService queryService, ICommandBus commandBus)
        {
            _queryService = queryService;
            _commandBus = commandBus;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuthorizedApplications()
        {
            var query = new GetAuthorizedApplicationsQuery()
            {
                CorrelationId = GetHeaderValue("x-correlationid"),
                TransactionId = GetHeaderValue("x-messageid")
            };
            var clusters = await _queryService.Query(query);
            if (clusters == null || !clusters.Any())
                return new NotFoundObjectResult("The user is not authorized to operate on any clusters");
            return new OkObjectResult(clusters);
        }

        [HttpGet]
        [Route("{cluster}/applications/{application}/keys")]
        public async Task<IActionResult> GetKeys([FromRoute] string cluster, [FromRoute] string application, [FromQuery] string search = "")
        {
            var query = new GetApplicationKeysQuery(cluster, application, search)
            {
                CorrelationId = GetHeaderValue("x-correlationid"),
                TransactionId = GetHeaderValue("x-messageid")
            };
            var keys = await _queryService.Query(query);
            if (keys == null || !keys.Any())
                return new NotFoundObjectResult("No keys are found in this application with the given search text");
            Response.Headers.Add("KeysCount", keys.Count.ToString());
            return new OkObjectResult(keys);
        }

        [HttpDelete]
        [Route("{cluster}/applications/{application}/keys")]
        public async Task<IActionResult> DeleteKeys([FromRoute] string cluster, [FromRoute] string application, [FromQuery] string search = "")
        {   
            bool.TryParse(GetHeaderValue("x-delete-secondary"), out bool deleteSecondary);
            var command = new FlushKeysCommand(cluster, application, search, deleteSecondary)
            {
                CorrelationId = GetHeaderValue("x-correlationid"),
                TransactionId = GetHeaderValue("x-messageid")
            };
            var result = await _commandBus.Send(command);
            if (result.DeletedKeys == null || !result.DeletedKeys.Any())
                return new NotFoundObjectResult("No keys are found in this application with the given search text");
            Response.Headers.Add("DeletedKeysCount", result.DeletedKeys.Count.ToString());
            return new OkObjectResult(result);
        }

        [HttpGet]
        [Route("{cluster}/applications/{application}/keys/{key}")]
        public async Task<IActionResult> GetKey([FromRoute] string cluster, [FromRoute] string application, [FromRoute] string key)
        {
            var query = new GetKeyQuery(cluster, application, key)
            {
                CorrelationId = GetHeaderValue("x-correlationid"),
                TransactionId = GetHeaderValue("x-messageid")
            };
            var unifiedRedisKey = await _queryService.Query(query);
            if (unifiedRedisKey == null)
                return new NotFoundObjectResult("Key not found");
            return new OkObjectResult(unifiedRedisKey);
        }

        [HttpDelete]
        [Route("{cluster}/applications/{application}/keys/{key}")]
        public async Task<IActionResult> DeleteKey([FromRoute] string cluster, [FromRoute] string application, [FromRoute] string key)
        {
            bool.TryParse(GetHeaderValue("x-delete-secondary"), out bool deleteSecondary);
            var command = new DeleteKeyCommand(cluster, application, key, deleteSecondary)
            {
                CorrelationId = GetHeaderValue("x-correlationid"),
                TransactionId = GetHeaderValue("x-messageid")
            };
            await _commandBus.Send(command);
            return new NoContentResult();
        }

        private string GetHeaderValue(string headerKey)
        {   
            if (!Request.Headers.Any(header => header.Key.ToLowerInvariant() == headerKey.ToLowerInvariant()))
                return null;
            var header = Request.Headers.FirstOrDefault(header => header.Key.ToLowerInvariant() == headerKey.ToLowerInvariant());
            return header.Value.FirstOrDefault();
        }
    }
}
