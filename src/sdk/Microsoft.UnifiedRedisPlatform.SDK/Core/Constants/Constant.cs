namespace Microsoft.UnifiedRedisPlatform.Core.Constants
{
    internal class Constant
    {
        public struct ExceptionMessages
        {
            public const string Unauthorized = "Invalid client secret is provided. Correlation ID: {0}";
            public const string Timeout = "The operation has execeeded the configured timeout. Correlation ID: {0}";
            public const string InvalidConfiguration = "Invalid Configuration. The following argument has invalid value: {0}";
        }

        public struct OperationApi
        {
            public const string DefaultUrl = "https://unified-redis-platform-api-prod.azurefd.net";
        }

        public struct LoggingConstants
        {
            public const string CacheAsideLogging = "CacheAsideLogging";
            public const string AggregateLogging = "AggregateLogging";
            public const string ImmediateLogging = "ImmediateLogging";

            public const string DefaultLogKey = "URP:Logs";
        }

        public struct WritePolicyConstants
        {
            public const string WriteThrough = "Write-Through";
            public const string DeleteThrough = "Delete-Through";
            public const string NotManaged = "Not-Managed";
        }
    }
}
