using Microsoft.CQRS;
using StackExchange.Redis;
using System.Threading.Tasks;
using AppInsights.EnterpriseTelemetry;
using AppInsights.EnterpriseTelemetry.Context;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;
using Microsoft.UnifiedPlatform.Service.Common.Authentication;
using AppInsights.EnterpriseTelemetry;

namespace Microsoft.UnifiedRedisPlatform.Manager.Domain.Commands.Handlers
{
    public class CreateKeyCommandHandler : CommandHandler<CreateKeyCommand, IdCommandResult>
    {
        private readonly IUnifiedConnectionMultiplexerFactory _muxFactory;
        private readonly IClusterConfigurationProvider _configurationProvider;
        private readonly IApplicationAuthenticator _authenticator;
        private readonly ILogger _logger;

        public CreateKeyCommandHandler(IUnifiedConnectionMultiplexerFactory muxFactory,
            IApplicationAuthenticator authenticator,
            IClusterConfigurationProvider configurationProvider,
            ILogger logger)
        {
            _muxFactory = muxFactory;
            _authenticator = authenticator;
            _configurationProvider = configurationProvider;
            _logger = logger;
        }

        protected override async Task<IdCommandResult> ProcessRequest(CreateKeyCommand request)
        {
            await _authenticator.EnsureAuthorized(request.Cluster, request.Application, request.CorrelationId, request.TransactionId);
            var appSecret = await _configurationProvider.GetApplicationSecret(request.Cluster, request.Application);
            var connectionMux = _muxFactory.Create(request.Cluster, request.Application, appSecret);
            var database = (connectionMux as IConnectionMultiplexer).GetDatabase();

            await database.StringSetAsync(request.Key, request.Value);
            LogEvent(request);
            return new IdCommandResult(request.Key);
        }

        private void LogEvent(CreateKeyCommand request)
        {
            var eventContext = new EventContext("Manager:Key:Created")
            {
                CorrelationId = request.CorrelationId,
                TransactionId = request.TransactionId
            };
            eventContext.AddProperty("Cluster", request.Cluster);
            eventContext.AddProperty("Application", request.Application);
            eventContext.AddProperty("Key", request.Key);
            eventContext.AddProperty("Valye", request.Value);
            _logger.Log(eventContext);
        }
    }
}
