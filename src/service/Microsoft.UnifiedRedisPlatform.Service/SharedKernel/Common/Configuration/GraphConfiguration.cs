namespace Microsoft.UnifiedPlatform.Service.Common.Configuration
{
    public class GraphConfiguration: BaseConfiguration
    {
        public string GraphEndpoint { get; set; }
        public string GroupDetailsUri { get; set; }
        public string GroupMembersUri { get; set; }

        public string GraphResourceId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public int MaxRetryCount { get; set; }
        public int CacheDurationInMins { get; set; }

        public override string Name => "Graph Configuration";
    }
}
