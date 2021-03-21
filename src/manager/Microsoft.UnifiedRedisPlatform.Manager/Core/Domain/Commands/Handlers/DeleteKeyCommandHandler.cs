using Microsoft.CQRS;
using StackExchange.Redis;
using System.Threading.Tasks;
using AppInsights.EnterpriseTelemetry;
using AppInsights.EnterpriseTelemetry.Context;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;
using Microsoft.UnifiedPlatform.Service.Common.Authentication;

namespace Microsoft.UnifiedRedisPlatform.Manager.Domain.Commands.Handlers
{
    public class DeleteKeyCommandHandler : CommandHandler<DeleteKeyCommand, IdCommandResult>
    {
        private readonly IUnifiedConnectionMultiplexerFactory _muxFactory;
        private readonly IClusterConfigurationProvider _configurationProvider;
        private readonly IApplicationAuthenticator _authenticator;
        private readonly ILogger _logger;

        public DeleteKeyCommandHandler(IUnifiedConnectionMultiplexerFactory muxFactory,
            IApplicationAuthenticator authenticator,
            IClusterConfigurationProvider configurationProvider,
            ILogger logger)
        {
            _muxFactory = muxFactory;
            _authenticator = authenticator;
            _configurationProvider = configurationProvider;
            _logger = logger;
        }

        protected override async Task<IdCommandResult> ProcessRequest(DeleteKeyCommand request)
        {
            await _authenticator.EnsureAuthorized(request.Cluster, request.Application, request.CorrelationId, request.TransactionId);
            var appSecret = await _configurationProvider.GetApplicationSecret(request.Cluster, request.Application);
            var connectionMux = _muxFactory.Create(request.Cluster, request.Application, appSecret);
            var database = (connectionMux as IConnectionMultiplexer).GetDatabase();

            #pragma warning disable CS0618 // Perf Issues are allowed in Management API
            await database.KeyDeleteAsync(request.KeyName);
            if (request.DeleteSecondary)
                await connectionMux.FlushSecondaryAsync(request.KeyName);
            #pragma warning restore CS0618 // Perf Issues are allowed in Management API

            LogEvent(request);
            return new IdCommandResult(request.KeyName);
        }

        private void LogEvent(DeleteKeyCommand request)
        {
            var eventContext = new EventContext("Manager:Key:Deleted")
            {
                CorrelationId = request.CorrelationId,
                TransactionId = request.TransactionId
            };
            eventContext.AddProperty("Cluster", request.Cluster);
            eventContext.AddProperty("Application", request.Application);
            eventContext.AddProperty("Key", request.KeyName);
            eventContext.AddProperty("ExplicitSecondaryDelete", request.DeleteSecondary.ToString());
            _logger.Log(eventContext);
        }
    }
}
