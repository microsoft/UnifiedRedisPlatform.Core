using System.Linq;
using System.Collections.Generic;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;

namespace Microsoft.UnifiedPlatform.Service.Configuration
{
    public class ConfigurationProviderChainBuilder: IConfigurationProviderChainBuilder
    {
        private readonly List<BaseConfigurationProvider> _confgurationProviders;

        /// <summary>
        /// Constructs the chain builder
        /// </summary>
        /// <param name="configurationProviders" cref="List{BaseConfigurationProvider}">List of configuration providers available in the system</param>
        public ConfigurationProviderChainBuilder(List<BaseConfigurationProvider> configurationProviders)
        {
            _confgurationProviders = configurationProviders;
        }

        /// <summary>
        /// Builds the chain from the confiuration providers passed in the constructor
        /// </summary>
        /// <returns cref="BaseConfigurationProvider">Gets the first configuration provider in the chain</returns>
        public BaseConfigurationProvider Build()
        {
            if (_confgurationProviders == null)
                return null;
            if (_confgurationProviders.Count == 0)
                return null;

            var prioritizedConfigurationProviders = _confgurationProviders.OrderBy(configProvider => configProvider.Priority).ToList();

            for (var iterator = 0; iterator < prioritizedConfigurationProviders.Count - 1; iterator++)
            {
                // Current provider should point to the next provider in the prioritized list
                prioritizedConfigurationProviders[iterator].Next = prioritizedConfigurationProviders[iterator + 1];
            }
            // The least prioritized configuration provider should not points to any providers
            prioritizedConfigurationProviders[prioritizedConfigurationProviders.Count - 1].Next = null;

            return prioritizedConfigurationProviders.FirstOrDefault();
        }
    }
}
