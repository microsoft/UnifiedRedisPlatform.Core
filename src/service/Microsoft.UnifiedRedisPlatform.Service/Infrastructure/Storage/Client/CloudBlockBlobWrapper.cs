using Azure.Storage.Blobs.Specialized;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.UnifiedPlatform.Storage.Client
{
    public class CloudBlockBlobWrapper : IBlockBlob
    {
        private readonly BlockBlobClient _baseBlob;

        public CloudBlockBlobWrapper(BlockBlobClient baseBlob)
        {
            _baseBlob = baseBlob;
        }

        public Task DownloadToStreamAsync(Stream target)
        {
            return _baseBlob.DownloadToAsync(target);
        }
    }
}
