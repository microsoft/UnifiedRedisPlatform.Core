using System;

namespace Microsoft.UnifiedPlatform.Service.Common.AppExceptions
{
    [Serializable]
    public class ConfigurationNotFoundException : Exception
    {
        public ConfigurationNotFoundException(string feature, string key)
            : base($"Configuration missing for feature - {feature} and key {key}") { }
    }
}
