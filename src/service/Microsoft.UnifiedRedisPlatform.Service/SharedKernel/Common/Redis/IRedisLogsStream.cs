using System.Threading.Tasks;
using System.Threading.Channels;
using System.Collections.Generic;
using Microsoft.UnifiedPlatform.Service.Common.Models;

namespace Microsoft.UnifiedPlatform.Service.Common.Redis
{
    public interface IRedisLogsStream
    {
        Task StreamLogs(ClusterConfigurationDto clusterConfiguration, AppConfigurationDto applicationConfiguration, Channel<List<Log>> logsChannel, int batchSize, bool commitLog);
    }
}
