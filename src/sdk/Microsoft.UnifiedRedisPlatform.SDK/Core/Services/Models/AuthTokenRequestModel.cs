namespace Microsoft.UnifiedRedisPlatform.Core.Services.Models
{
    internal class AuthTokenRequestModel
    {   
        public string ClusterName { get; set; }
        public string AppName { get; set; }

        public AuthTokenRequestModel(string clusterName, string appName)
        {
            ClusterName = clusterName;
            AppName = appName;
        }
    }
}
