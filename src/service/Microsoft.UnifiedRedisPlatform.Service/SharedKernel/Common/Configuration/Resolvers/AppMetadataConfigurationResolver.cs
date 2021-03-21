namespace Microsoft.UnifiedPlatform.Service.Common.Configuration.Resolvers
{
    public class AppMetadataConfigurationResolver : IConfigurationResolver<AppMetadataConfiguration>
    {
        private readonly BaseConfigurationProvider _configurationProvider;

        public AppMetadataConfigurationResolver(BaseConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public AppMetadataConfiguration Resolve()
        {
            return new AppMetadataConfiguration()
            {
                Environment = _configurationProvider.GetConfiguration("Application", "EnvironmentName").Result,
                ServiceOffering = _configurationProvider.GetConfiguration("Application", "ServiceOffering").Result,
                ServiceLine = _configurationProvider.GetConfiguration("Application", "ServiceLine").Result,
                Service = _configurationProvider.GetConfiguration("Application", "Service").Result,
                Capability = _configurationProvider.GetConfiguration("Application", "Capability").Result,
                ComponentName = _configurationProvider.GetConfiguration("Application", "Component").Result,
                ComponentId = _configurationProvider.GetConfiguration("Application", "ComponentId").Result,
                ICTO_ID = _configurationProvider.GetConfiguration("Application", "IctoId").Result,

                TenantNameHeaderKey = _configurationProvider.GetConfiguration("Application", "TenantNameHeaderKey").Result,
                CorrelationIdHeaderKey = _configurationProvider.GetConfiguration("Application", "CorrelationIdHeaderKey").Result,
                TransactionIdHeaderKey = _configurationProvider.GetConfiguration("Application", "TransactionIdHeaderKey").Result,
                SecretHeaderKey = _configurationProvider.GetConfiguration("Application", "SecretsHeaderKey").Result,
                PreferredLocationHeaderKey = _configurationProvider.GetConfiguration("Application", "PreferredLocationHeaderKey").Result,

                ClusterClaimsKey = _configurationProvider.GetConfiguration("Claims", "ClusterKey").Result,
                AppClaimsKey = _configurationProvider.GetConfiguration("Claims", "AppKey").Result,

                AdministratorGroup = _configurationProvider.GetConfiguration("Application", "AdministratorGroup").Result,

                Region = new RegionConfiguration()
                {
                    Name = _configurationProvider.GetConfiguration("Application", "Region:Name").Result,
                    DisplayName = _configurationProvider.GetConfiguration("Application", "Region:DisplayName").Result
                }
                
            };
        }
    }
}
