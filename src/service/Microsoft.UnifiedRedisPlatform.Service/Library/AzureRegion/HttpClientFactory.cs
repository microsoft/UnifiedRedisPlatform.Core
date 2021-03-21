using System.Net.Http;

namespace Microsoft.AzureRegion
{
    internal interface IHttpClientFactory
    {
        HttpClient Create();
    }

    internal class HttpClientFactory : IHttpClientFactory
    {
        public static HttpClient HttpClient;
        public static object lockObj = new object();

        public HttpClient Create()
        {
            lock (lockObj)
            {
                if (HttpClient != null)
                    return HttpClient;
                var retryHandler = new RetryHttpClientHandler(new HttpClientHandler());
                HttpClient = new HttpClient(retryHandler);
                return HttpClient;
            }
        }
    }
}
