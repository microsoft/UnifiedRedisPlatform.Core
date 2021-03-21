namespace Microsoft.UnifiedRedisPlatform.Core.Services.Models
{
    internal class AuthTokenResponseModel
    {
        public string Token { get; set; }
        public string Resource { get; set; }
        public long ExpiresOn { get; set; }
    }
}
