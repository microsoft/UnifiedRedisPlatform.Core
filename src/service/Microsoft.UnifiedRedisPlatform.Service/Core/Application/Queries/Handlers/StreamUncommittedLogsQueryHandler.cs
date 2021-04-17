using CQRS.Mediatr.Lite;
using System.Threading.Tasks;
using Microsoft.UnifiedPlatform.Service.Common.Redis;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;

namespace Microsoft.UnifiedPlatform.Service.Application.Queries.Handlers
{
    public class StreamUncommittedLogsQueryHandler : QueryHandler<StreamUncommitedLogsQuery, bool>
    {
        private readonly IRedisLogsStream _redisListProvider;
        private readonly IClusterConfigurationProvider _configurationProvider;

        public StreamUncommittedLogsQueryHandler(IRedisLogsStream redisListProvider, IClusterConfigurationProvider configurationProvider)
        {
            _redisListProvider = redisListProvider;
            _configurationProvider = configurationProvider;
        }

        protected override async Task<bool> ProcessRequest(StreamUncommitedLogsQuery request)
        {
            var clusterConfiguration = await _configurationProvider.GetClusterDetails(request.ClusterName);
            var appConfiguration = await _configurationProvider.GetApplicationDetails(request.ClusterName, request.AppName);
            clusterConfiguration.ConnectionStrings = await _configurationProvider.GetClusterConnectionStrings(request.ClusterName, request.AppName);

            await _redisListProvider.StreamLogs(clusterConfiguration, appConfiguration, request.LogChannel, request.BatchSize, commitLog: true);
            return true;
        }
    }
}
