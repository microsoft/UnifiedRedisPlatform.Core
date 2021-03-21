using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace CommonUtilities
{
    public class SecretProvider
    {
        private readonly KeyVaultClient _client;

        public SecretProvider(string keyVaultName)
        {
            var keyVaultUri = $"https://{keyVaultName}.vault.azure.net";
            var tokenProvider = new AzureServiceTokenProvider();
            _client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(tokenProvider.KeyVaultTokenCallback));
        }

        public async Task<string> GetSecret(string secretName)
        {
            var secretBundle = await _client.GetSecretAsync(secretName);
            return secretBundle.Value;
        }
    }
}
