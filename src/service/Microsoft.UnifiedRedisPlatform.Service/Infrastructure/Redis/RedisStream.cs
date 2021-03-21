using System.Linq;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Threading.Channels;
using System.Collections.Generic;
using Microsoft.UnifiedPlatform.Service.Common.Redis;
using Microsoft.UnifiedPlatform.Service.Common.Models;

namespace Microsoft.UnifiedPlatform.Service.Redis
{
    public class RedisStream : IRedisLogsStream
    {
        private readonly IRedisConnectionManager _redisConnectionManager;

        public RedisStream(IRedisConnectionManager redisConnectionManager)
        {
            _redisConnectionManager = redisConnectionManager;
        }

        public async Task StreamLogs(ClusterConfigurationDto clusterConfiguration, AppConfigurationDto applicationConfiguration, Channel<List<Log>> logsChannel, int batchSize, bool commitLog)
        {
            var appKey = $"{clusterConfiguration.RedisCachePrefix}:{applicationConfiguration.RedisCachePrefix}";
            var logKey = applicationConfiguration.DiagnosticSettings != null && !string.IsNullOrWhiteSpace(applicationConfiguration.DiagnosticSettings.LogKey)
                ? applicationConfiguration.DiagnosticSettings.LogKey
                : "URP:Logs";

            var listKey = $"{appKey}:{logKey}";
            for (int connStrIdx = 0; connStrIdx < clusterConfiguration.ConnectionStrings.Count; connStrIdx++)
            {
                bool isLastConnStr = connStrIdx == clusterConfiguration.ConnectionStrings.Count - 1;
                await StreamLogsFromRedisConnection(clusterConfiguration.ConnectionStrings[connStrIdx].ConnectionString, listKey, logsChannel, batchSize, commitLog, closeChannel: isLastConnStr);
            }
        }

        private async Task StreamLogsFromRedisConnection(string redisConnectionString, string listKey, Channel<List<Log>> logsChannel, int batchSize, bool commitLog, bool closeChannel)
        {
            var connection = _redisConnectionManager.CreateConnection(redisConnectionString);
            var database = connection.GetDatabase();
            ChannelWriter<List<Log>> logsChannelWriter = logsChannel.Writer;

            var logLength = await database.ListLengthAsync(listKey);
            if (logLength <= 0)
            {
                if (closeChannel)
                    logsChannelWriter.Complete();
                return;
            }
            
            var readCursor = 0;
            var endCursor = batchSize;
            do
            {   
                RedisValue[] redisLogs = await database.ListRangeAsync(listKey, readCursor, endCursor);
                if (redisLogs == null || !redisLogs.Any())
                    break;

                List<Log> logs = redisLogs.Select(redisLog => JsonConvert.DeserializeObject<Log>(redisLog.ToString())).ToList();

                while (await logsChannelWriter.WaitToWriteAsync())
                {
                    await logsChannelWriter.WriteAsync(logs);
                    break;
                }

                if (commitLog)
                    await database.ListTrimAsync(listKey, endCursor + 1, -1);
            } while (true);

            if (closeChannel)
                logsChannelWriter.Complete();

            await connection.CloseAsync();
        }
    }
}
