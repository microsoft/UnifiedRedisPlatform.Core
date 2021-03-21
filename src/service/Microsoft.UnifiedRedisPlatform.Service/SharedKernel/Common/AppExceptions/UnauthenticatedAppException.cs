using System;

namespace Microsoft.UnifiedPlatform.Service.Common.AppExceptions
{
    [Serializable]
    public class UnauthenticatedAppException: Exception
    {
        public UnauthenticatedAppException(string clusterName, string appName)
            : base($"The secret provider for app - {appName} in cluster {clusterName} is invalid") { }
    }
}
