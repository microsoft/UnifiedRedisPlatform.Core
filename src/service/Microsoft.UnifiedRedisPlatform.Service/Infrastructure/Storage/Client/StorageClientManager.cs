using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;

namespace Microsoft.UnifiedPlatform.Storage.Client
{
    public class StorageClientManager : IStorageClientManager
    {
        private readonly CloudStorageAccount _storageAccount;

        public StorageClientManager(StorageConfiguration configuration)
        {
            var credentials = new StorageCredentials(configuration.StorageAccountName, configuration.StorageAccountKey);
            _storageAccount = new CloudStorageAccount(credentials, useHttps: true);
        }

        public async Task<ITable> CreateTable(string tableName)
        {
            var tableClient = _storageAccount.CreateCloudTableClient();
            var cloudTable = tableClient.GetTableReference(tableName);
            await cloudTable.CreateIfNotExistsAsync();
            return new CloudTableWrapper(cloudTable);
        }

        public async Task<IBlockBlob> CreateBlob(string containerName, string blobName)
        {
            var blobClient = _storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(containerName);
            await blobContainer.CreateIfNotExistsAsync();

            var blob = blobContainer.GetBlockBlobReference(blobName);
            return new CloudBlockBlobWrapper(blob);
        }
        
    }
}
