using StackExchange.Redis;
using System.Collections.Generic;
using Microsoft.UnifiedPlatform.Service.Common.Redis;

namespace Microsoft.UnifiedPlatform.Service.Redis
{
    public class RedisConnectionManger: IRedisConnectionManager
    {
        public static Dictionary<string, IConnectionMultiplexer> Multiplexers = new Dictionary<string, IConnectionMultiplexer>();

        public IConnectionMultiplexer CreateConnection(string connectionString)
        {
            if (Multiplexers.ContainsKey(connectionString))
            {
                var mux = Multiplexers[connectionString];
                if (mux.IsConnected)
                    return mux;
                mux = ConnectionMultiplexer.Connect(connectionString);
                return mux;
            }
            var newMux = ConnectionMultiplexer.Connect(connectionString);
            Multiplexers.Add(connectionString, newMux);
            return newMux;
                
        }
    }
}
