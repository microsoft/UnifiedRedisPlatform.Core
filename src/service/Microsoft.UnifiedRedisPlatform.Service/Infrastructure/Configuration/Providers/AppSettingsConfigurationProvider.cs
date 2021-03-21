using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;

namespace Microsoft.UnifiedPlatform.Service.Configuration.Providers
{
    public class AppSettingsConfigurationProvider: BaseConfigurationProvider
    {
        private readonly IConfiguration _configuration;
        private readonly int _priority;
        /// <summary>
        /// Constructs the configuration provider
        /// </summary>
        /// <param name="configuration" cref="IConfiguration">.NET Core configuration interface</param>
        public AppSettingsConfigurationProvider(IConfiguration configuration, int priority = 1)
        {
            _priority = priority;
            _configuration = configuration;
        }

        public override int Priority => _priority;

        protected override Task<string> HandleConfigurationRequest(string feature, string key)
        {
            var configurationKey = string.IsNullOrEmpty(feature) ? key : $"{feature}:{key}";
            var configurationValue = _configuration[configurationKey];
            return Task.FromResult(configurationValue);
        }
    }
}
