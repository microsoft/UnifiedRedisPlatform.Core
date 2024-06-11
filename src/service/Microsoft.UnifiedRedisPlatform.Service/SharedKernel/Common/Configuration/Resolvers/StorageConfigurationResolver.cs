using System;

namespace Microsoft.UnifiedPlatform.Service.Common.Configuration.Resolvers
{
    public class StorageConfigurationResolver : IConfigurationResolver<StorageConfiguration>
    {
        private readonly BaseConfigurationProvider _configurationProvider;
        private readonly BaseConfigurationProvider _secretConfigurationProvider;

        public StorageConfigurationResolver(BaseConfigurationProvider appSettingsConfigurationProvider, BaseConfigurationProvider secretConfigurationProvider)
        {
            _configurationProvider = appSettingsConfigurationProvider;
            _secretConfigurationProvider = secretConfigurationProvider;
        }

        public StorageConfiguration Resolve()
        {
            return new StorageConfiguration()
            {
                StorageAccountName = _configurationProvider.GetConfiguration("Storage", "Name").Result,                
                ConfigurationTableName = _configurationProvider.GetConfiguration("Storage", "ConfigurationTable").Result,                    
                BackoffInternal = TimeSpan.FromMilliseconds(int.Parse(_configurationProvider.GetConfiguration("Storage", "BackoffInterval").Result)),
                MaxAttempt = int.Parse(_configurationProvider.GetConfiguration("Storage", "MaxAttempt").Result)
            };
        }
    }
}
