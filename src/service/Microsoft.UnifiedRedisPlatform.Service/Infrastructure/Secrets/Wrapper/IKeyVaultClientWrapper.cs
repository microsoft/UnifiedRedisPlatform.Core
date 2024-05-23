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
    public interface IKeyVaultClientWrapper
    {
        /// <summary>
        /// Wraps the GetSecretAsync by secret name method
        /// </summary>
        /// <param name="key">Secret key</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>KeyVault Secret</returns>
        Task<Response<KeyVaultSecret>> GetSecretAsync(string key, CancellationToken token = default(CancellationToken));
    }
}
