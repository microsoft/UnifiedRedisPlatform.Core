using System;
using Autofac;
using System.Linq;
using CQRS.Mediatr.Lite;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AppInsights.EnterpriseTelemetry;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.UnifiedPlatform.Service.Application.Queries;

namespace Function
{
    public class GetConfigurations
    {
        private readonly IQueryService _queryService;
        private readonly ILogger _logger;

        public GetConfigurations(IContainer container, ILogger logger)
        {
            _queryService = container.Resolve<IQueryService>();
            _logger = logger;
        }

        [FunctionName("Configurations")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            try
            {
                var query = new GetAllClustersQuery();
                var clusters = await _queryService.Query(query);
                if (clusters == null || !clusters.Any())
                    return new NotFoundObjectResult("No clusters have been onboarded in this environment");
                return new OkObjectResult(clusters);
            }
            catch (Exception error)
            {
                _logger.Log(error, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "GetConfigurations.Run", "System", null);
                throw;
            }
        }
    }
}
