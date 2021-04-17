using System;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.UnifiedRedisPlatform.Core.Logging;
using Microsoft.UnifiedRedisPlatform.Core.Pipeline;

namespace Microsoft.UnifiedRedisPlatform.Core.RedisExecutionMiddlewares
{
    internal class ResilliencyMiddleware : BaseMiddleware
    {
        private readonly UnifiedConfigurationOptions _configurations;
        private readonly Random _randomGenerator;
        private readonly ILogger _logger;

        public ResilliencyMiddleware(UnifiedConfigurationOptions configurations, ILogger logger)
        {
            _configurations = configurations;
            _logger = logger;
            _randomGenerator = new Random();
        }

        public override T Execute<T>(Func<T> action, ExecutionContext context)
        {
            return ExecuteWithRetry<T>(action, context as RedisExecutionContext, maxRetry: GetMaxRetryCount(context), retryCount: 1);
        }

        public override async Task<T> ExecuteAsync<T>(Func<Task<T>> action, ExecutionContext context)
        {
            return await ExecuteWithRetryAsync<T>(action, context as RedisExecutionContext, maxRetry: GetMaxRetryCount(context), 1);
        }

        private T ExecuteWithRetry<T>(Func<T> action, RedisExecutionContext context, int maxRetry, int retryCount = 1)
        {
            try
            {
                return action();
            }
            catch (Exception exception)
            {
                return RetryOperation(action, context, maxRetry, retryCount, exception);
            }
        }

        private async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> action, RedisExecutionContext context, int maxRetry, int retryCount = 1)
        {
            try
            {
                return await action();
            }
            catch (Exception exception)
            {
                return await RetryOperationAsync(action, context, maxRetry, retryCount, exception);
            }
        }

        private T RetryOperation<T>(Func<T> action, RedisExecutionContext context, int maxRetry, int retryCount, Exception exception)
        {
            var isRedisException = exception is RedisException || exception is RedisTimeoutException;

            // Don't retry when general exception is thrown
            if (!isRedisException)
            {
                _logger.LogException(exception);
                _logger.LogEvent($"OperationFailure:{action.Method.Name}", properties: new Dictionary<string, string>() { { "Error", exception.Message }, { "Operation", action.Method.Name } });
                throw exception;
            }

            var methodName = action.Method.Name;

            if (retryCount > maxRetry)
            {
                _logger.LogException(exception);
                _logger.LogEvent($"OperationFailure:{methodName}", properties: CreateErrorProperty(exception, methodName, retryCount));
                throw exception;
            }

            var wait = _randomGenerator.Next(GetMinBackoffInterval(context), GetMaxBackoffInterval(context));

            var loggerTask = Task.Run(() =>
            {
                _logger.LogException(exception);
                _logger.LogEvent($"OperationFailureRetry:{methodName}", properties: CreateErrorProperty(exception, methodName, retryCount));
            });

            Task.WaitAll(Task.Delay(wait), loggerTask);
            return ExecuteWithRetry(action, context, retryCount + 1);
        }

        private async Task<T> RetryOperationAsync<T>(Func<Task<T>> action, RedisExecutionContext context, int maxRetry, int retryCount, Exception exception)
        {
            var isRedisException = exception is RedisException || exception is RedisTimeoutException;

            // Don't retry when general exception is thrown
            if (!isRedisException)
            {
                _logger.LogException(exception);
                _logger.LogEvent($"OperationFailure:{action.Method.Name}", properties: new Dictionary<string, string>() { { "Error", exception.Message }, { "Operation", action.Method.Name } });
                throw exception;
            }

            var methodName = action.Method.Name;

            if (retryCount > maxRetry)
            {
                _logger.LogException(exception);
                _logger.LogEvent($"OperationFailure:{methodName}", properties: CreateErrorProperty(exception, methodName, retryCount));
                throw exception;
            }

            var wait = _randomGenerator.Next(GetMinBackoffInterval(context), GetMaxBackoffInterval(context));

            var loggerTask = Task.Run(() =>
            {
                _logger.LogException(exception);
                _logger.LogEvent($"OperationFailureRetry:{methodName}", properties: CreateErrorProperty(exception, methodName, retryCount));
            });

            await Task.WhenAll(Task.Delay(wait), loggerTask);
            return await ExecuteWithRetryAsync(action, context, retryCount + 1);
        }

        private int GetMaxRetryCount(ExecutionContext context)
        {
            if (context is RedisExecutionContext redisContext && redisContext.MaxRetry > 0)
                return redisContext.MaxRetry;
            return _configurations.OperationsRetryProtocol.MaxRetryCount;
        }

        private int GetMaxBackoffInterval(RedisExecutionContext redisContext)
        {
            return redisContext != null && redisContext.MaxBackoffInterval > 0
                ? redisContext.MaxBackoffInterval
                : _configurations.OperationsRetryProtocol.MaxBackoffIntervalInMs;
        }

        private int GetMinBackoffInterval(RedisExecutionContext redisContext)
        {
            return redisContext != null && redisContext.MaxBackoffInterval > 0
                ? redisContext.MaxBackoffInterval
                : _configurations.OperationsRetryProtocol.MaxBackoffIntervalInMs;
        }

        private Dictionary<string, string> CreateErrorProperty(Exception exception, string methodName, int retryCount)
        {
            return new Dictionary<string, string>()
            {
                { "Error", exception.Message },
                { "Operation", methodName },
                { "RetryCount", retryCount.ToString() }
            };
        }
    }
}
