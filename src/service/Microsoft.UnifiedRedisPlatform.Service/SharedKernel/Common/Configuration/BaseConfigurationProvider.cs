using System.Threading.Tasks;
using Microsoft.UnifiedPlatform.Service.Common.AppExceptions;

namespace Microsoft.UnifiedPlatform.Service.Common.Configuration
{
    public abstract class BaseConfigurationProvider
    {
        /// <summary>
        /// Gets configuration from a configuration source
        /// </summary>
        /// <param name="feature">Configuration group (based on feature)</param>
        /// <param name="key">Key of the configuration</param>
        /// <returns>Configuration Value</returns>
        protected abstract Task<string> HandleConfigurationRequest(string feature, string key);

        public abstract int Priority { get; }

        /// <summary>
        /// The next Configuration Provider in the chain
        /// </summary>
        public BaseConfigurationProvider Next;

        /// <summary>
        /// Handles the request for getting a configuration
        /// </summary>
        /// <param name="feature">Configuration group (based on feature)</param>
        /// <param name="key">Key of the configuration</param>
        /// <returns>Configuration Value</returns>
        /// <exception cref="ConfigurationNotFoundException">When configuration is not found and no more configuration is chained</exception>
        public virtual async Task<string> GetConfiguration(string feature, string key)
        {
            var configurationValue = await HandleConfigurationRequest(feature, key);
            if (configurationValue != null)
                return configurationValue;

            if (Next == null)
                throw new ConfigurationNotFoundException(feature, key);

            return await Next.GetConfiguration(feature, key);
        }
    }
}
