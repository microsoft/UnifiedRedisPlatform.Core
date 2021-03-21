using System.Threading.Tasks;

namespace Microsoft.UnifiedPlatform.Storage.Client
{
    public interface IStorageClientManager
    {
        Task<ITable> CreateTable(string tableName);
        Task<IBlockBlob> CreateBlob(string containerName, string blobName);
    }
}