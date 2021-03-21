using System.Linq;
using StackExchange.Redis;
using System.Collections.Generic;
using Microsoft.UnifiedRedisPlatform.Core.Logging;
using Microsoft.UnifiedRedisPlatform.Core.Database;

namespace Microsoft.UnifiedRedisPlatform.Core
{
    public partial class UnifiedConnectionMultiplexer
    {
        public IDatabase GetDatabase(int db = -1, object asyncState = null)
        {
            IDatabase primaryDatabase = _baseConnectionMux.GetDatabase(db, asyncState);
            IEnumerable<IDatabase> secondaryDatabases = _secondaryConnectionMuxs.Select(mux => mux.GetDatabase(db, asyncState));
            return new UnifiedRedisDatabase(primaryDatabase, secondaryDatabases, this, _unifiedConfigurations, _logger);
        }

        public IDatabase GetDatabase(RedisExecutionContext context, int db = -1, object asyncState = null)
        {
            IDatabase primaryDatabase = _baseConnectionMux.GetDatabase(db, asyncState);
            IEnumerable<IDatabase> secondaryDatabases = _secondaryConnectionMuxs.Select(mux => mux.GetDatabase(db, asyncState));
            return new UnifiedRedisDatabase(primaryDatabase, secondaryDatabases, this, _unifiedConfigurations, _logger, context);
        }

        private IDatabase GetLoggingDatabase()
        {
            UnifiedConfigurationOptions loggingConfiguration = _unifiedConfigurations.Clone() as UnifiedConfigurationOptions;
            loggingConfiguration.DiagnosticSettings.Enabled = false;
            loggingConfiguration.OperationsRetryProtocol.HardTimeoutEnabled = false;
            return new UnifiedRedisDatabase(_baseConnectionMux.GetDatabase(), null, this, loggingConfiguration, BlankLogger.Get());
        }

        private ILogger CreateStrategicLogger(ILogger clientLogger)
        {
            var serviceAggregatedLogger = new ServiceAggregatedLogger(_serviceEndpoint, ClusterName, AppName, AppSecret, _unifiedConfigurations.Region, _unifiedConfigurations.DiagnosticSettings);
            var serviceLogger = new ServiceLogger(_serviceEndpoint, ClusterName, AppName, AppSecret, _unifiedConfigurations.Region, _unifiedConfigurations.DiagnosticSettings);
            var cacheLogger = new CacheLogger(GetLoggingDatabase(), _unifiedConfigurations.DiagnosticSettings.LogKey);
            return new StrategicLogger(clientLogger, cacheLogger, serviceAggregatedLogger, serviceLogger, _unifiedConfigurations);
        }
    }
}
