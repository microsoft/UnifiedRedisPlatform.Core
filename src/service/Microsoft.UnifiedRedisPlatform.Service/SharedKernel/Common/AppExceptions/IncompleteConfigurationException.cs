using System;

namespace Microsoft.UnifiedPlatform.Service.Common.AppExceptions
{
    [Serializable]
    public class IncompleteConfigurationException: Exception
    {
        public IncompleteConfigurationException(string clusterName, string appName, string missingConfiguration, Exception innerException)
            : base($"For app {appName} in cluster {clusterName}, the following configuration is missing: {missingConfiguration}", innerException) { }
    }
}
