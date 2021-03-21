using System.IO;
using System.Threading.Tasks;

namespace Microsoft.UnifiedPlatform.Storage.Client
{
    public interface IBlockBlob
    {
        Task DownloadToStreamAsync(Stream target);
    }
}