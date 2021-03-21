using System.Threading.Tasks;
using Microsoft.UnifiedPlatform.Service.Common.Secrets;
using Microsoft.UnifiedPlatform.Service.Common.AppExceptions;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;

namespace Microsoft.UnifiedPlatform.Service.Configuration.Providers
{
    public class SecretsConfigurationProvider: BaseConfigurationProvider
    {
        private readonly ISecretsProvider _secretsProvider;
        private readonly int _priority;

        /// <summary>
        /// Constructs the Secret Configuration Provider
        /// </summary>
        /// <param name="secretsProvider" cref="ISecretsProvider">Gets secret from the secret provider like Key Vault</param>
        public SecretsConfigurationProvider(ISecretsProvider secretsProvider, int priority = 100)
        {
            _secretsProvider = secretsProvider;
            _priority = priority;
        }

        public override int Priority => _priority;

        protected override async Task<string> HandleConfigurationRequest(string feature, string key)
        {
            var configurationKey = string.IsNullOrEmpty(feature) ? key : $"{feature}-{key}";
            try
            {
                return await _secretsProvider.GetSecret(configurationKey);
            }
            catch (SecretNotFoundException)
            {
                return null;
            }
        }
    }
}
