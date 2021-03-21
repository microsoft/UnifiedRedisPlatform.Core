using Autofac;
using AppInsights.EnterpriseTelemetry;
using Microsoft.Extensions.Configuration;
using AppInsights.EnterpriseTelemetry.Web.Extension;
using Microsoft.UnifiedRedisPlatform.Service.Dependencies.DependencyResolution;

namespace Microsoft.UnifiedRedisPlatform.Service.Function.DependencyResolution
{
    public class DependencyResolver : CommonDependencyModule
    {
        public DependencyResolver(IConfiguration configuration)
            : base(configuration)
        { }

        protected override void RegisterApplicationInsights(ContainerBuilder builder)
        {
            builder.Register(scope =>
            {
                var logger = EnterpriseTelemetryExtensions.CreateLogger(_configuration);
                return logger;
            }).As<ILogger>()
            .SingleInstance();
        }
    }
}
