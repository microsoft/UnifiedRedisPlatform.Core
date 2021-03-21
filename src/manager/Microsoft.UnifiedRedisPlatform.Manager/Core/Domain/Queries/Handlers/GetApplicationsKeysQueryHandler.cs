using System.Linq;
using Microsoft.CQRS;
using System.Threading.Tasks;
using AppInsights.EnterpriseTelemetry;
using System.Collections.Generic;
using AppInsights.EnterpriseTelemetry.Context;
using Microsoft.UnifiedPlatform.Service.Common.Models;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;
using Microsoft.UnifiedPlatform.Service.Common.Authentication;

namespace Microsoft.UnifiedRedisPlatform.Manager.Domain.Queries.Handlers
{
    public class GetApplicationsKeysQueryHandler : QueryHandler<GetApplicationKeysQuery, List<UnifiedRedisKey>>
    {
        private readonly IUnifiedConnectionMultiplexerFactory _muxFactory;
        private readonly IClusterConfigurationProvider _configurationProvider;
        private readonly IApplicationAuthenticator _authenticator;
        private readonly ILogger _logger;

        public GetApplicationsKeysQueryHandler(IUnifiedConnectionMultiplexerFactory muxFactory, 
            IApplicationAuthenticator authenticator,
            IClusterConfigurationProvider configurationProvider, 
            ILogger logger)
        {
            _muxFactory = muxFactory;
            _authenticator = authenticator;
            _configurationProvider = configurationProvider;
            _logger = logger;
        }

        protected override async Task<List<UnifiedRedisKey>> ProcessRequest(GetApplicationKeysQuery request)
        {
            await _authenticator.EnsureAuthorized(request.Cluster, request.Application, request.CorrelationId, request.TransactionId);
            var appSecret = await _configurationProvider.GetApplicationSecret(request.Cluster, request.Application);
            var connectionMux = _muxFactory.Create(request.Cluster, request.Application, appSecret);

            #pragma warning disable CS0618 // Perf Issues are allowed in Management API
            var keys = await connectionMux.GetKeysAsync(string.IsNullOrWhiteSpace(request.SearchText) ? "" : request.SearchText);
            #pragma warning restore CS0618 // Perf Issues are allowed in Management API

            LogEvent(request, keys);
            if (keys == null || !keys.Any())
                return null;

            return keys.Select(key => new UnifiedRedisKey(key.ToString())).ToList();
        }

        private void LogEvent(GetApplicationKeysQuery request, List<StackExchange.Redis.RedisKey> keys)
        {
            var eventContext = new EventContext("Manager:Keys:Retrieved")
            {
                CorrelationId = request.CorrelationId,
                TransactionId = request.TransactionId
            };
            eventContext.AddProperty("Cluster", request.Cluster);
            eventContext.AddProperty("Application", request.Application);
            eventContext.AddProperty("Count", keys != null ? keys.Count.ToString() : "0");
            eventContext.AddProperty("SearchPattern", request.SearchText);
            _logger.Log(eventContext);
        }
    }
}
