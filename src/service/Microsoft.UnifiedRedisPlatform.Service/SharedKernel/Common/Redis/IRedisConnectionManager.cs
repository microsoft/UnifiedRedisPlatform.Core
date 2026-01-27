using StackExchange.Redis;
using System.Threading.Tasks;

namespace Microsoft.UnifiedPlatform.Service.Common.Redis
{
    public interface IRedisConnectionManager
    {
        Task<IConnectionMultiplexer> CreateConnectionAsync(string connectionString);
    }
}
