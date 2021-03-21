using System.Threading.Tasks;

namespace Microsoft.UnifiedPlatform.Service.Common.Storage
{
    public interface IBlobReader
    {
        Task<string> Read(string containerName, string blobName, string correlationId, string transactionId);
    }
}
