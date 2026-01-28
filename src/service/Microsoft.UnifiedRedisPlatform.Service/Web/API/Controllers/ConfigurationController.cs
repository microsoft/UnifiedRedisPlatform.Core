using System.Linq;
using Microsoft.CQRS;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using AppInsights.EnterpriseTelemetry.Web.Extension.Filters;
using Microsoft.UnifiedPlatform.Service.Application.Queries;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;

namespace Microsoft.UnifiedRedisPlatform.Service.API.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(RequestResponseLoggerFilterAttribute))]
    [Route("api/configurations")]
    public class ConfigurationController: BaseController
    {
        private readonly IQueryService _queryService;
        public ConfigurationController(AppMetadataConfiguration configuration, IQueryService queryService)
            :base(configuration)
        {
            _queryService = queryService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var clusterName = GetClusterFromClaims();
            var appName = GetAppFromClaims();
            var preferredLocaltion = GetPreferredLocation();
            var useManagedIdentity = Request.Headers["x-use-managed-identity"].FirstOrDefault()?.Equals("true", System.StringComparison.OrdinalIgnoreCase) ?? false;

            var query = new GetClusterConfigurationQuery(clusterName, appName, preferredLocaltion, useManagedIdentity)
            {
                CorrelationId = GetCorrelationId(),
                TransactionId = GetTransactionId()
            };

            var configuration = await _queryService.Query(query);
            return new OkObjectResult(configuration);
        }
    }
}
