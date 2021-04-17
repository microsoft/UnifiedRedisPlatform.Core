using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.UnifiedRedisPlatform.Core.Logging;
using Microsoft.UnifiedRedisPlatform.Core.Pipeline;

namespace Microsoft.UnifiedRedisPlatform.Core.RedisExecutionMiddlewares
{
    internal class DiagnosticMiddleware : BaseMiddleware
    {
        private readonly ILogger _logger;
        private readonly UnifiedConfigurationOptions _configurations;
        public DiagnosticMiddleware(UnifiedConfigurationOptions configuration, ILogger logger)
        {
            _configurations = configuration;
            _logger = logger;
        }

        public override T Execute<T>(Func<T> action, ExecutionContext context)
        {
            if (!IsDiagnosticsEnabled(context))
                return _next.Execute(action, context);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = _next.Execute(action, context);
            stopwatch.Start();

            _logger.LogEvent($"Operation:Success:{action.Method.Name}", stopwatch.ElapsedMilliseconds);

            return result;
        }

        public override async Task<T> ExecuteAsync<T>(Func<Task<T>> action, ExecutionContext context)
        {
            if (!IsDiagnosticsEnabled(context))
                return await _next.ExecuteAsync(action, context);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = await _next.ExecuteAsync(action, context);
            stopwatch.Start();

            _logger.LogEvent($"Operation:Success:{action.Method.Name}", stopwatch.ElapsedMilliseconds);

            return result;
        }

        public static bool IsDiagnosticsEnabled(ExecutionContext context)
        {
            if (!(context is RedisExecutionContext redisContext))
                return true;

            return redisContext.IsDiagnosticEnabled;
        }
    }
}
