using Microsoft.UnifiedRedisPlatform.Core;

namespace Microsoft.UnifiedRedisPlatform.Manager.Domain
{
    public interface IUnifiedConnectionMultiplexerFactory
    {
        IUnifiedConnectionMultiplexer Create(string clusterName, string applicationName, string appSecret);
    }

    public class UnifiedConnectionMultiplexerFactory : IUnifiedConnectionMultiplexerFactory
    {
        private readonly string _managedIdentityClientId;

        public UnifiedConnectionMultiplexerFactory(string managedIdentityClientId = null)
        {
            _managedIdentityClientId = managedIdentityClientId;
        }

        public IUnifiedConnectionMultiplexer Create(string clusterName, string applicationName, string appSecret)
        {
            return UnifiedConnectionMultiplexer.Connect(clusterName, applicationName, appSecret, managedIdentityClientId: _managedIdentityClientId) as IUnifiedConnectionMultiplexer;
        }
    }
}
