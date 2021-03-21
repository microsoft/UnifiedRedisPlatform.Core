namespace Microsoft.UnifiedRedisPlatform.FunctionalTests
{
    public class TokenRequest
    {
        public string ClusterName { get; set; }
        public string AppName { get; set; }

        public TokenRequest(string clusterName, string appName)
        {
            ClusterName = clusterName;
            AppName = appName;
        }
    }
}
