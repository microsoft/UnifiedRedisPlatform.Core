using AppInsights.EnterpriseTelemetry.AppInsightsInitializers;
using AppInsights.EnterpriseTelemetry.Web.Extension;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UnifiedRedisPlatform.Service.Function.DependencyResolution;
using System;

[assembly: FunctionsStartup(typeof(Microsoft.UnifiedRedisPlatform.Service.Function.Startup))]

namespace Microsoft.UnifiedRedisPlatform.Service.Function
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();

            builder.ConfigurationBuilder
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{context.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {   
            builder.Services.AddEnterpriseTelemetry(builder.GetContext().Configuration);
            builder.Services.AddSingleton<ITelemetryInitializer, ResponseCodeTranslationIntitializer>();
            builder.Services.AddSingleton<ITelemetryInitializer, ClientSideErrorInitializer>();
            IContainer applicationContainer = RegisterDependencies(builder.Services, builder.GetContext().Configuration);
            builder.Services.AddSingleton(sp => applicationContainer);
        }


        protected virtual IContainer RegisterDependencies(IServiceCollection services, IConfiguration configuration)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new DependencyResolver(configuration));
            containerBuilder.Populate(services);
            return containerBuilder.Build();
        }
    }
}
