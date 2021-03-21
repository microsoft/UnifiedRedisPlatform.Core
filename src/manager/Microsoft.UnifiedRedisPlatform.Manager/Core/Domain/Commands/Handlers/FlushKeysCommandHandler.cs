using System.Linq;
using Microsoft.CQRS;
using System.Threading.Tasks;
using AppInsights.EnterpriseTelemetry;
using System.Collections.Generic;
using AppInsights.EnterpriseTelemetry.Context;
using Microsoft.UnifiedPlatform.Service.Common.Models;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;
using Microsoft.UnifiedPlatform.Service.Common.Authentication;
using Microsoft.UnifiedRedisPlatform.Manager.Domain.Commands.Results;

namespace Microsoft.UnifiedRedisPlatform.Manager.Domain.Commands.Handlers
{
    public class FlushKeysCommandHandler : CommandHandler<FlushKeysCommand, KeysResult>
    {
        private readonly IUnifiedConnectionMultiplexerFactory _muxFactory;
        private readonly IClusterConfigurationProvider _configurationProvider;
        private readonly IApplicationAuthenticator _authenticator;
        private readonly ILogger _logger;

        public FlushKeysCommandHandler(IUnifiedConnectionMultiplexerFactory muxFactory,
            IApplicationAuthenticator authenticator,
            IClusterConfigurationProvider configurationProvider,
            ILogger logger)
        {
            _muxFactory = muxFactory;
            _authenticator = authenticator;
            _configurationProvider = configurationProvider;
            _logger = logger;
        }

        protected override async Task<KeysResult> ProcessRequest(FlushKeysCommand request)
        {
            await _authenticator.EnsureAuthorized(request.Cluster, request.Application, request.CorrelationId, request.TransactionId);
            var appSecret = await _configurationProvider.GetApplicationSecret(request.Cluster, request.Application);
            var connectionMux = _muxFactory.Create(request.Cluster, request.Application, appSecret);

            #pragma warning disable CS0618 // Perf Issues are allowed in Management API
            var keys = await connectionMux.FlushAsync(string.IsNullOrWhiteSpace(request.SearchText) ? "" : request.SearchText);
            if (request.DeleteSecondary)
                await connectionMux.FlushSecondaryAsync(string.IsNullOrWhiteSpace(request.SearchText) ? "" : request.SearchText);
            #pragma warning restore CS0618 // Perf Issues are allowed in Management API

            LogEvent(request, keys);
            if (keys == null || !keys.Any())
                return null;

            var deletedKeys =  keys.Select(key => new UnifiedRedisKey(key.ToString())).ToList();
            return new KeysResult(deletedKeys);
        }

        private void LogEvent(FlushKeysCommand request, List<StackExchange.Redis.RedisKey> deletedKeys)
        {
            var eventContext = new EventContext("Manager:Keys:Deleted")
            {
                CorrelationId = request.CorrelationId,
                TransactionId = request.TransactionId
            };
            eventContext.AddProperty("Cluster", request.Cluster);
            eventContext.AddProperty("Application", request.Application);
            eventContext.AddProperty("ExplicitSecondaryFlush", request.DeleteSecondary.ToString());
            eventContext.AddProperty("Count", deletedKeys != null ? deletedKeys.Count.ToString() : "0");
            eventContext.AddProperty("Keys", string.Join(';', deletedKeys));
            eventContext.AddProperty("SearchPattern", request.SearchText);
            _logger.Log(eventContext);
        }
    }
}
