using Microsoft.Extensions.Configuration;
using Microsoft.UnifiedRedisPlatform.Service.Dependencies.DependencyResolution;

namespace Microsoft.UnifiedRedisPlatform.Service.API.DependencyResolution
{
    public class DependencyResolver : CommonDependencyModule
    {
        public DependencyResolver(IConfiguration configuration)
            : base(configuration)
        { }
    }
}
