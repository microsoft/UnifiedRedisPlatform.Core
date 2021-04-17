using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.UnifiedRedisPlatform.Core.Logging;
using Microsoft.UnifiedRedisPlatform.Core.Pipeline;
using ExecutionContext = Microsoft.UnifiedRedisPlatform.Core.Pipeline.ExecutionContext;

namespace Microsoft.UnifiedRedisPlatform.Core.RedisExecutionMiddlewares
{
    internal class HardTimeoutMiddleware : BaseMiddleware
    {
        private readonly UnifiedConfigurationOptions _configuration;
        private readonly ILogger _logger;

        public HardTimeoutMiddleware(UnifiedConfigurationOptions configurations, ILogger logger)
        {
            _configuration = configurations;
            _logger = logger;
        }

        public override T Execute<T>(Func<T> action, ExecutionContext context)
        {
            if (!IsHardTimeoutEnabled(context))
                return _next.Execute(action, context);

            var hardTimeout = GetTimeout(context);
            T result = default;
            var isTaskCompleted = Task.Run(() =>
            {
                result = _next.Execute(action, context);
            }).Wait(hardTimeout);

            if (!isTaskCompleted)
                throw HandleTimeout(action.Method.Name, context, hardTimeout);

            return result;
        }

        public override async Task<T> ExecuteAsync<T>(Func<Task<T>> action, ExecutionContext context)
        {
            if (!IsHardTimeoutEnabled(context))
                return await _next.ExecuteAsync(action, context);

            var hardTimeout = GetTimeout(context);
            var delayTask = Task.Delay(hardTimeout);
            var executionTask = _next.ExecuteAsync(action, context);
            await Task.WhenAny(delayTask, executionTask);

            if (executionTask.IsCompleted)
                return executionTask.Result;

            throw HandleTimeout(action.Method.Name, context, hardTimeout);
        }


        private Exception HandleTimeout(string operationName, ExecutionContext context, int hardTimeout)
        {   
            var exception = new TimeoutException(context != null ? context.CorrelationId : Guid.NewGuid().ToString());
            var logProperties = new Dictionary<string, string>()
            {
                { "Operation", operationName },
                { "WaitTime",  hardTimeout.ToString() }
            };
            _logger.LogException(exception, logProperties);
            return exception;
        }

        private int GetTimeout(ExecutionContext context)
        {
            if (context is RedisExecutionContext redisContext && redisContext.TimeoutInMs > 0)
                return redisContext.TimeoutInMs;
            return _configuration.OperationsRetryProtocol.TimeoutInMs;
        }

        private static bool IsHardTimeoutEnabled(ExecutionContext context)
        {
            if (!(context is RedisExecutionContext redisContext))
                return true;

            return redisContext.IsHardTimeoutEnabled;
        }
    }
}
