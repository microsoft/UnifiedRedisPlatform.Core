using Azure.Data.Tables;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;
using Microsoft.UnifiedPlatform.Service.Common.Helpers;
using System;
using System.Threading.Tasks;

namespace Microsoft.UnifiedPlatform.Storage.Client
{
    public class StorageClientManager : IStorageClientManager
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly TableServiceClient _tableServiceClient;
        private readonly IDefaultAzureCredentialProvider _defaultAzureCredentialProvider;

        public StorageClientManager(IDefaultAzureCredentialProvider defaultAzureCredentialProvider, StorageConfiguration configuration)
        {
            _defaultAzureCredentialProvider = defaultAzureCredentialProvider;
            var storageaccountName = configuration?.StorageAccountName;
           
            // Create BlobServiceClient with managed identity
            _blobServiceClient = new BlobServiceClient(new Uri(string.Format("https://{0}.blob.core.windows.net", storageaccountName)), _defaultAzureCredentialProvider.GetDefaultAzureCredential());
            // Create TableServiceClient with managed identity
            _tableServiceClient = new TableServiceClient(new Uri(string.Format("https://{0}.table.core.windows.net", storageaccountName)), _defaultAzureCredentialProvider.GetDefaultAzureCredential());
        }

        public async Task<ITable> CreateTable(string tableName)
        {
            var tableClient = _tableServiceClient.GetTableClient(tableName);
            await tableClient.CreateIfNotExistsAsync();
            return new CloudTableWrapper(tableClient);
        }


        public async Task<IBlockBlob> CreateBlob(string containerName, string blobName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.CreateIfNotExistsAsync();

            var blobClient = blobContainerClient.GetBlockBlobClient(blobName);
            return new CloudBlockBlobWrapper(blobClient);
        }

    }
}
