using System;
using CQRS.Mediatr.Lite;

namespace Microsoft.UnifiedPlatform.Service.Application.Commands
{
    public class AuthenticateClientCommand : Command<TokenCommandResult>
    {
        public override string DisplayName => "Authenticate Client";

        private readonly string _id;
        public override string Id => _id;

        public string ClusterName { get; set; }
        public string AppName { get; set; }
        public string AppSecret { get; set; }

        public AuthenticateClientCommand(string clusterName, string appName, string appSecret)
        {
            _id = Guid.NewGuid().ToString();
            ClusterName = clusterName;
            AppName = appName;
            AppSecret = appSecret;
        }

        public override bool Validate(out string ValidationErrorMessage)
        {
            ValidationErrorMessage = null;

            if (string.IsNullOrWhiteSpace(ClusterName))
                ValidationErrorMessage = "Cluster Name is missing";

            if (string.IsNullOrWhiteSpace(AppName))
                ValidationErrorMessage = "App Name is missing";

            if (string.IsNullOrWhiteSpace(AppSecret))
                ValidationErrorMessage = "App Secret is missing";

            return string.IsNullOrWhiteSpace(ValidationErrorMessage);
        }
    }

    public class TokenCommandResult : CommandResult
    {
        public string Token { get; set; }
        public string Resource { get; set; }
        public long ExpiresOn { get; set; }

        public TokenCommandResult(string token, string resource, long expiresOn) : base(true)
        {
            Token = token;
            Resource = resource;
            ExpiresOn = expiresOn;
        }
    }
}
