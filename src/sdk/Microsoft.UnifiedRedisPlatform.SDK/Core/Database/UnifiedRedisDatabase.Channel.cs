using StackExchange.Redis;
using System.Threading.Tasks;

namespace Microsoft.UnifiedRedisPlatform.Core.Database
{
    public partial class UnifiedRedisDatabase
    {
        public long Publish(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None) =>
            _primaryDatabase.Publish(channel, message, flags);

        public Task<long> PublishAsync(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None) =>
            _primaryDatabase.PublishAsync(channel, message, flags);
    }
}
