using System.IO;
using System.Text;
using System.Threading.Tasks;
using AppInsights.EnterpriseTelemetry;
using Microsoft.UnifiedPlatform.Storage.Client;
using Microsoft.UnifiedPlatform.Service.Common.Storage;
using AppInsights.EnterpriseTelemetry.Context;

namespace Microsoft.UnifiedPlatform.Storage
{
    public class BlobReader: IBlobReader
    {
        private readonly IStorageClientManager _storageClientManager;
        private readonly ILogger _logger;

        public BlobReader(IStorageClientManager storageClientManager, ILogger logger)
        {
            _storageClientManager = storageClientManager;
            _logger = logger;
        }

        public async Task<string> Read(string containerName, string blobName, string correlationId, string transactionId)
        {
            var performanceContext = new PerformanceContext("BlobReader:Read")
            {
                CorrelationId = correlationId,
                TransactionId = transactionId
            };
            performanceContext.Start();

            var blobText = string.Empty;
            var blob = await _storageClientManager.CreateBlob(containerName, blobName);
            using (var memoryStream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(memoryStream);
                blobText = Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            performanceContext.Stop();
            _logger.Log(performanceContext);

            return blobText;
        }
    }
}
