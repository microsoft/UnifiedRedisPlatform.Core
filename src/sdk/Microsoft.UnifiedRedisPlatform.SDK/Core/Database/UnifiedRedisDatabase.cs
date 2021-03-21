using System;
using System.Linq;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.UnifiedRedisPlatform.Core.Logging;
using Microsoft.UnifiedRedisPlatform.Core.Pipeline;
using Microsoft.UnifiedRedisPlatform.Core.Constants;
using Microsoft.UnifiedRedisPlatform.Core.RedisExecutionMiddlewares;

namespace Microsoft.UnifiedRedisPlatform.Core.Database
{
    public partial class UnifiedRedisDatabase : IUnifiedDatabase
    {
        private readonly IDatabase _primaryDatabase;
        private readonly IEnumerable<IDatabase> _secondaryDatabases;

        private readonly List<IDatabase> _writeDatabases;
        private readonly List<IDatabase> _deleteDatabases;

        private readonly UnifiedConnectionMultiplexer _unifiedMux;
        private readonly UnifiedConfigurationOptions _configurations;
        private readonly BaseMiddleware _executionManager;
        private readonly RedisExecutionContext _context;
        private readonly ILogger _logger;

        public UnifiedRedisDatabase(IDatabase rootDatabase, IEnumerable<IDatabase> secondardDatabases, UnifiedConnectionMultiplexer mux, UnifiedConfigurationOptions configurations, ILogger logger)
            : this(rootDatabase, secondardDatabases, mux, configurations, logger, null) { }

        public UnifiedRedisDatabase(IDatabase primaryDatabase, IEnumerable<IDatabase> secondaryDatabases, UnifiedConnectionMultiplexer mux, UnifiedConfigurationOptions configurations, ILogger logger, RedisExecutionContext context)
        {
            _primaryDatabase = primaryDatabase;
            _secondaryDatabases = secondaryDatabases;
            _unifiedMux = mux;
            _configurations = configurations;
            _logger = logger;

            _writeDatabases = new List<IDatabase>() { _primaryDatabase };
            _deleteDatabases = new List<IDatabase>() { _primaryDatabase };

            if (_secondaryDatabases != null && _secondaryDatabases.Any() && !string.IsNullOrWhiteSpace(_configurations.WritePolicy))
            {   
                if (_configurations.WritePolicy.ToLowerInvariant() == Constant.WritePolicyConstants.WriteThrough.ToLowerInvariant())
                {
                    _writeDatabases.AddRange(_secondaryDatabases);
                    _deleteDatabases.AddRange(_secondaryDatabases);
                }
                else if (_configurations.WritePolicy.ToLowerInvariant() == Constant.WritePolicyConstants.DeleteThrough.ToLowerInvariant())
                {
                    _deleteDatabases.AddRange(_secondaryDatabases);
                }
            }

            IPipeline executionPipeline = new PipelineManager();

            if (_configurations.OperationsRetryProtocol != null && _configurations.OperationsRetryProtocol.HardTimeoutEnabled)
                executionPipeline = executionPipeline.With(new HardTimeoutMiddleware(_configurations, _logger));

            if (_configurations.DiagnosticSettings.Enabled)
                executionPipeline = executionPipeline.With(new DiagnosticMiddleware(_configurations, _logger));

            executionPipeline.With(new ResilliencyMiddleware(_configurations, _logger));
            _executionManager = executionPipeline.Create();
            _context = context;
        }

        public int Database => _primaryDatabase.Database;

        public IConnectionMultiplexer Multiplexer { get => _unifiedMux; }

        private RedisKey CreateAppKey(RedisKey rootKey)
        {
            var appKey = $"{_configurations.KeyPrefix}:{rootKey}";
            return appKey;
        }

        private RedisKey[] CreateAppKeys(RedisKey[] rootKeys)
        {
            return rootKeys.Select(key => CreateAppKey(key)).ToArray();
        }

        public void EnableHardTimeout()
        {
            _configurations.OperationsRetryProtocol.HardTimeoutEnabled = true;
        }

        public void DisableHardTimeout()
        {
            _configurations.OperationsRetryProtocol.HardTimeoutEnabled = false;
        }

        public TimeSpan Ping(CommandFlags flags = CommandFlags.None) => _primaryDatabase.Ping(flags);

        public Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None) => _primaryDatabase.PingAsync(flags);

        protected Task<T> ExecuteAsync<T>(Func<Task<T>> action)
        {
            return _executionManager.ExecuteAsync<T>(action, _context);
        }

        protected T Execute<T>(Func<T> action)
        {
            return _executionManager.Execute<T>(action, _context);
        }

        protected List<T> ExecuteWrite<T>(IEnumerable<Func<T>> actions)
        {
            var results = new List<T>();
            foreach (var action in actions)
            {
                results.Add(_executionManager.Execute<T>(action, _context));
            }
            return results;
        }

        protected bool ExecuteWrite(IEnumerable<Func<bool>> actions)
        {
            var results = new List<bool>();
            foreach (var action in actions)
            {
                results.Add(_executionManager.Execute<bool>(action, _context));
            }
            return results.All(res => res);
        }

        protected List<Task<T>> ExecuteWriteAsync<T>(IEnumerable<Func<Task<T>>> actions)
        {
            var tasks = new List<Task<T>>();
            foreach (var action in actions)
            {
                tasks.Add(_executionManager.ExecuteAsync(action, _context));
            }
            return tasks;
        }

        protected async Task<bool> ExecuteWriteAsync(IEnumerable<Func<Task<bool>>> actions)
        {
            var tasks = new List<Task<bool>>();
            foreach (var action in actions)
            {
                tasks.Add(_executionManager.ExecuteAsync(action, _context));
            }
            await Task.WhenAll(tasks);
            return tasks.Select(task => task.Result).All(res => res);
        }
    }
}
