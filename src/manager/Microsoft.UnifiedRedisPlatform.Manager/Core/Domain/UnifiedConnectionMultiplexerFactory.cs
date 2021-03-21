using Microsoft.UnifiedRedisPlatform.Core;

namespace Microsoft.UnifiedRedisPlatform.Manager.Domain
{
    public interface IUnifiedConnectionMultiplexerFactory
    {
        IUnifiedConnectionMultiplexer Create(string clusterName, string applicationName, string appSecret);
    }

    public class UnifiedConnectionMultiplexerFactory : IUnifiedConnectionMultiplexerFactory
    {
        public IUnifiedConnectionMultiplexer Create(string clusterName, string applicationName, string appSecret)
        {
            return UnifiedConnectionMultiplexer.Connect(clusterName, applicationName, appSecret) as IUnifiedConnectionMultiplexer;
        }
    }
}
