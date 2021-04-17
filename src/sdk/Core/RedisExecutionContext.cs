using Microsoft.UnifiedRedisPlatform.Core.Pipeline;

namespace Microsoft.UnifiedRedisPlatform.Core
{
    public class RedisExecutionContext: ExecutionContext
    {
        public bool IsDiagnosticEnabled { get; set; }
        public bool IsHardTimeoutEnabled { get; set; }
        public int MaxRetry { get; set; }
        public int TimeoutInMs { get; set; }
        public int MaxBackoffInterval { get; set; }
        public int MinBackoffInterval { get; set; }
    }
}
