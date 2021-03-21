using System;

namespace Microsoft.UnifiedRedisPlatform.Core.Pipeline
{
    public class ExecutionContext
    {
        private string _correlationId;
        public string CorrelationId { get => _correlationId ?? Guid.NewGuid().ToString(); set { _correlationId = value; } }

        private string _transactionId;
        public string TransactionId { get => _transactionId ?? Guid.NewGuid().ToString(); set { _transactionId = value; } }
    }
}
