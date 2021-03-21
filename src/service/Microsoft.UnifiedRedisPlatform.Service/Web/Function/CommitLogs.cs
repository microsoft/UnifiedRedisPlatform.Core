using System;
using Autofac;
using System.Linq;
using Microsoft.CQRS;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Channels;
using System.Collections.Generic;
using AppInsights.EnterpriseTelemetry;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.UnifiedPlatform.Service.Common.Models;
using Microsoft.UnifiedPlatform.Service.Application.Queries;
using Microsoft.UnifiedPlatform.Service.Application.Commands;

namespace Microsoft.UnifiedRedisPlatform.Service.Function
{
    public class CommitLogs
    {
        private readonly IQueryService _queryService;
        private readonly ICommandBus _commandBus;
        private readonly ILogger _logger;


        public CommitLogs(IContainer container, ILogger logger)
        {
            _queryService = container.Resolve<IQueryService>();
            _commandBus = container.Resolve<ICommandBus>();
            _logger = logger;
        }

        [FunctionName("Log")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            try
            {
                var clusterName = req.Query.ContainsKey("Cluster") ? req.Query["Cluster"].FirstOrDefault() : null;
                var appName = req.Query.ContainsKey("App") ? req.Query["App"].FirstOrDefault() : null;
                Channel<List<Log>> logsBatchChannel = Channel.CreateUnbounded<List<Log>>();
                await Task.WhenAll(new Task[] { SendLogs(_commandBus, logsBatchChannel, clusterName, appName), StreamLogs(_queryService, logsBatchChannel, clusterName, appName) });
                return new OkObjectResult("Logs Committed");
            }
            catch (Exception exception)
            {
                _logger.Log(exception, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "SendUncommittedLogs.Run", "System", "N/A");
                throw;
            }
        }

        private async Task SendLogs(ICommandBus commandBus, Channel<List<Log>> logChannel, string clusterName, string appName)
        {
            ChannelReader<List<Log>> logsBatchChannelReader = logChannel.Reader;
            while (await logsBatchChannelReader.WaitToReadAsync())
            {
                List<Log> batch = await logsBatchChannelReader.ReadAsync();
                if (batch != null || !batch.Any())
                {
                    var logCommand = new LogClientInfoCommand(batch, clusterName, appName);
                    await commandBus.Send(logCommand);
                }
            }
        }

        private async Task StreamLogs(IQueryService queryService, Channel<List<Log>> logChannel, string clusterName, string appName)
        {
            var getLogsQuery = new StreamUncommitedLogsQuery(clusterName, appName, logChannel, batchSize: 500);
            await queryService.Query(getLogsQuery);
        }
    }
}
