using System;
using Microsoft.CQRS;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;
using Microsoft.UnifiedPlatform.Service.Common.AppExceptions;
using Microsoft.UnifiedPlatform.Service.Common.Authentication;

namespace Microsoft.UnifiedPlatform.Service.Application.Commands.Handlers
{
    public class AuthenticateClientCommandHandler : CommandHandler<AuthenticateClientCommand, TokenCommandResult>
    {
        private readonly IAuthenticator _authenticator;
        private readonly IClusterConfigurationProvider _clusterConfigurationProvider;

        public AuthenticateClientCommandHandler(IAuthenticator authenticator, IClusterConfigurationProvider clusterConfigurationProvider)
        {
            _authenticator = authenticator;
            _clusterConfigurationProvider = clusterConfigurationProvider;
        }

        protected override async Task<TokenCommandResult> ProcessRequest(AuthenticateClientCommand request)
        {
            var appDetails = await _clusterConfigurationProvider.GetApplicationDetails(request.ClusterName, request.AppName);
            if (appDetails == null)
                throw new InvalidAppException(request.ClusterName, request.AppName);

            var appSecret = await _clusterConfigurationProvider.GetApplicationSecret(request.ClusterName, request.AppName);
            if (!request.AppSecret.Equals(appSecret))
                throw new UnauthenticatedAppException(request.ClusterName, request.AppName);

            var claims = new Dictionary<string, string>()
            {
                { "cluster", request.ClusterName },
                { "app", request.AppName }
            };

            var authToken = await _authenticator.GenerateToken(request.AppName, claims);
            return new TokenCommandResult(authToken, request.AppName, DateTime.UtcNow.AddMinutes(45).Ticks);
        }
    }
}
