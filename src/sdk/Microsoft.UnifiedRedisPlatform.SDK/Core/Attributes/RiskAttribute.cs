using System;

namespace Microsoft.UnifiedRedisPlatform.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    internal sealed class RiskAttribute: Attribute
    {
        public RiskAttribute(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
