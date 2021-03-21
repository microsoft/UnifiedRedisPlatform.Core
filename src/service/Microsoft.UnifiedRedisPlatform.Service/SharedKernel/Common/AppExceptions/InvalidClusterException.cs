using System;

namespace Microsoft.UnifiedPlatform.Service.Common.AppExceptions
{
    [Serializable]
    public class InvalidClusterException: Exception
    {
        public InvalidClusterException(string clusterKey)
            : base($"Cluster: {clusterKey} has not been registered") { }

        public InvalidClusterException(string clusterKey, Exception innerException)
            : base($"Cluster: {clusterKey} has not been registered", innerException) { }
    }
}
