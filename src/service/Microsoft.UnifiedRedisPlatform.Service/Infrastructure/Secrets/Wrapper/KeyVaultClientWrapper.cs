using Azure;
using Azure.Security.KeyVault.Secrets;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.UnifiedPlatform.Service.Secrets.Wrapper
{
    /// <summary>
    /// Wrapper around the KeyVaultClient
    /// </summary>
    /// <remarks>Used for help for mocking in unit testing</remarks>
    public class KeyVaultClientWrapper : IKeyVaultClientWrapper
    {
        private readonly SecretClient _secretClient;
        private readonly string _keyVaultBaseUri;

        /// <summary>
        /// Constructs the wrapper
        /// </summary>
        /// <param name="keyVaultBaseUri">The base URL of the Key Vault</param>
        /// <param name="keyVaultClient">The actual client for interfacing with KeyVault</param>
        public KeyVaultClientWrapper(string keyVaultBaseUri, SecretClient secretClient)
        {
            _keyVaultBaseUri = keyVaultBaseUri;
            _secretClient = secretClient;
        }

        /// <summary>
        /// Wraps the GetSecretAsync by secret name method
        /// </summary>
        /// <param name="key">Secret key</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>KeyVault Secret</returns>
        public Task<Response<KeyVaultSecret>> GetSecretAsync(string key, CancellationToken token = default(CancellationToken))
        {
            return _secretClient.GetSecretAsync(key, null, token);
        }
    }
}
