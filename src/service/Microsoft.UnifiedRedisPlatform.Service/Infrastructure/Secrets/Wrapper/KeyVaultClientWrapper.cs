using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;

namespace Microsoft.UnifiedPlatform.Service.Secrets.Wrapper
{
    /// <summary>
    /// Wrapper around the KeyVaultClient
    /// </summary>
    /// <remarks>Used for help for mocking in unit testing</remarks>
    public class KeyVaultClientWrapper : IKeyVaultClientWrapper
    {
        private readonly KeyVaultClient _keyVaultClient;
        private readonly string _keyVaultBaseUri;

        /// <summary>
        /// Constructs the wrapper
        /// </summary>
        /// <param name="keyVaultBaseUri">The base URL of the Key Vault</param>
        /// <param name="keyVaultClient">The actual client for interfacing with KeyVault</param>
        public KeyVaultClientWrapper(string keyVaultBaseUri, KeyVaultClient keyVaultClient)
        {
            _keyVaultBaseUri = keyVaultBaseUri;
            _keyVaultClient = keyVaultClient;
        }

        /// <summary>
        /// Wraps the GetSecretAsync by secret name method
        /// </summary>
        /// <param name="key">Secret key</param>
        /// <param name="token" cref="CancellationToken">Cancellation token</param>
        /// <returns cref="Task{SecretBundle}">Secret Bundle</returns>
        public Task<SecretBundle> GetSecretAsync(string key, CancellationToken token = default(CancellationToken))
        {
            return _keyVaultClient.GetSecretAsync(_keyVaultBaseUri, key, token);
        }
    }
}
