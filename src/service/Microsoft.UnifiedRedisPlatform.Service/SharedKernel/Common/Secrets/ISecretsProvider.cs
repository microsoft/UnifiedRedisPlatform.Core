using System.Threading.Tasks;

namespace Microsoft.UnifiedPlatform.Service.Common.Secrets
{
    public interface ISecretsProvider
    {
        Task<string> GetSecret(string key);
    }
}
