using Microsoft.CQRS;
using StackExchange.Redis;
using System.Threading.Tasks;
using AppInsights.EnterpriseTelemetry;
using AppInsights.EnterpriseTelemetry.Context;
using Microsoft.UnifiedPlatform.Service.Common.Models;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;
using Microsoft.UnifiedPlatform.Service.Common.Authentication;

namespace Microsoft.UnifiedRedisPlatform.Manager.Domain.Queries.Handlers
{
    public class GetKeyQueryHandler : QueryHandler<GetKeyQuery, UnifiedRedisKey>
    {
        private readonly IUnifiedConnectionMultiplexerFactory _muxFactory;
        private readonly IClusterConfigurationProvider _configurationProvider;
        private readonly IApplicationAuthenticator _authenticator;
        private readonly ILogger _logger;

        public GetKeyQueryHandler(IUnifiedConnectionMultiplexerFactory muxFactory,
            IApplicationAuthenticator authenticator,
            IClusterConfigurationProvider configurationProvider,
            ILogger logger)
        {
            _muxFactory = muxFactory;
            _authenticator = authenticator;
            _configurationProvider = configurationProvider;
            _logger = logger;
        }
        protected override async Task<UnifiedRedisKey> ProcessRequest(GetKeyQuery request)
        {
            await _authenticator.EnsureAuthorized(request.Cluster, request.Application, request.CorrelationId, request.TransactionId);
            var appSecret = await _configurationProvider.GetApplicationSecret(request.Cluster, request.Application);
            var connectionMux = _muxFactory.Create(request.Cluster, request.Application, appSecret);
            var database = (connectionMux as IConnectionMultiplexer).GetDatabase();
            var value = await database.StringGetAsync (request.Key);
            LogEvent(request, value);
            return string.IsNullOrWhiteSpace(value) ? null : new UnifiedRedisKey(request.Key, value);
        }

        private void LogEvent(GetKeyQuery request, string value)
        {
            var eventContext = new EventContext("Manager:Key:Retrieved")
            {
                CorrelationId = request.CorrelationId,
                TransactionId = request.TransactionId
            };
            eventContext.AddProperty("Cluster", request.Cluster);
            eventContext.AddProperty("Application", request.Application);
            eventContext.AddProperty("Key", request.Key);
            eventContext.AddProperty("Value", value);
            _logger.Log(eventContext);
        }
    }
}
