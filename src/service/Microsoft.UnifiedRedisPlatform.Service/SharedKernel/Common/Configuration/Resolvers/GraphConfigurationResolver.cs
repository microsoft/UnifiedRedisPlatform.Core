namespace Microsoft.UnifiedPlatform.Service.Common.Configuration.Resolvers
{
    public class GraphConfigurationResolver : IConfigurationResolver<GraphConfiguration>
    {
        private readonly BaseConfigurationProvider _configurationProvider;

        public GraphConfigurationResolver(BaseConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public GraphConfiguration Resolve()
        {
            return new GraphConfiguration()
            {
                GraphEndpoint = _configurationProvider.GetConfiguration("Graph", "Endpoint").Result,
                GraphResourceId = _configurationProvider.GetConfiguration("Graph", "ResourceId").Result,
                ClientId = _configurationProvider.GetConfiguration("Authentication", "AAD:Audience").Result,
                ClientSecret = _configurationProvider.GetConfiguration("Authentication", "AAD-Secret").Result,
                GroupDetailsUri = _configurationProvider.GetConfiguration("Graph", "GroupDetailsUri").Result,
                GroupMembersUri = _configurationProvider.GetConfiguration("Graph", "GroupMembersUri").Result,
                MaxRetryCount = int.Parse(_configurationProvider.GetConfiguration("Graph", "MaxRetryCount").Result),
                CacheDurationInMins = int.Parse(_configurationProvider.GetConfiguration("Graph", "CacheDurationInMins").Result),
            };
        }
    }
}
