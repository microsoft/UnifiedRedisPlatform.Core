using StackExchange.Redis;

namespace Microsoft.UnifiedPlatform.Service.Common.Redis
{
    public interface IRedisConnectionManager
    {
        IConnectionMultiplexer CreateConnection(string connectionString);
    }
}
