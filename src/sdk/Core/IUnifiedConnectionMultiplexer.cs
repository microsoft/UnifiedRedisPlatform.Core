using System;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Microsoft.UnifiedRedisPlatform.Core
{
    /// <summary>
    /// Represents the abstract multiplexer API for Unified Connections
    /// </summary>
    public interface IUnifiedConnectionMultiplexer: IConnectionMultiplexer
    {
        string ClusterName { get; }
        string AppName { get; }

        [Obsolete("May cause performance issues. Please avoid using this method.")]
        Task<List<RedisKey>> GetKeysAsync(string pattern = "");

        [Obsolete("May cause performance issues. Please avoid using this method.")]
        List<RedisKey> GetKeys(string pattern = "");

        [Obsolete("May cause performance issues. Please avoid using this method.")]
        List<RedisKey> Flush(string pattern = "", CommandFlags flags = CommandFlags.None);
        
        [Obsolete("May cause performance issues. Please avoid using this method.")]
        Task<List<RedisKey>> FlushAsync(string pattern = "", CommandFlags flags = CommandFlags.None);

        [Obsolete("May cause performance issues. Please avoid using this method.")]
        void FlushSecondary(string pattern = "", CommandFlags flags = CommandFlags.None);

        [Obsolete("May cause performance issues. Please avoid using this method.")]
        Task FlushSecondaryAsync(string pattern = "", CommandFlags flags = CommandFlags.None);
    }
}
