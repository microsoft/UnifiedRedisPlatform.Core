using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Microsoft.UnifiedPlatform.Storage.Client
{
    public class CloudBlockBlobWrapper : IBlockBlob
    {
        private readonly CloudBlockBlob _baseBlob;

        public CloudBlockBlobWrapper(CloudBlockBlob baseBlob)
        {
            _baseBlob = baseBlob;
        }

        public Task DownloadToStreamAsync(Stream target)
        {
            return _baseBlob.DownloadToStreamAsync(target);
        }
    }
}
