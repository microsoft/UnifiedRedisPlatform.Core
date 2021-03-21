using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault.Models;

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
        /// <param name="token" cref="CancellationToken">Cancellation token</param>
        /// <returns cref="Task{SecretBundle}">Secret Bundle</returns>
        Task<SecretBundle> GetSecretAsync(string key, CancellationToken token = default(CancellationToken));
    }
}
