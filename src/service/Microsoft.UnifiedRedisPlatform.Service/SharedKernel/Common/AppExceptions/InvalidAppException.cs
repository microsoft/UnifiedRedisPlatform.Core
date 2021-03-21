using System;

namespace Microsoft.UnifiedPlatform.Service.Common.AppExceptions
{
    [Serializable]
    public class InvalidAppException: Exception
    {
        public InvalidAppException(string clusterName, string appName) 
            : base($"The app with name {appName} has not been registered under cluster {clusterName}") { }

        public InvalidAppException(string clusterName, string appName, Exception innerException)
            : base($"The app with name {appName} has not been registered under cluster {clusterName}", innerException) { }
    }
}
