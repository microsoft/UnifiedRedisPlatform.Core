using CQRS.Mediatr.Lite;
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

            var query = new GetClusterConfigurationQuery(clusterName, appName, preferredLocaltion)
            {
                CorrelationId = GetCorrelationId(),
                TransactionId = GetTransactionId()
            };

            var configuration = await _queryService.Query(query);
            return new OkObjectResult(configuration);
        }
    }
}
