using Msit.Telemetry.Extensions.AI;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;
using Microsoft.UnifiedPlatform.Service.Telemetry.Initializers;

namespace Microsoft.UnifiedPlatform.Service.Telemetry.Client
{

    public class AppInsightsClientManager : IAppInsightsClientManager
    {
        public IAppInsightsTelemetryClientWrapper CreateClient(ApplicationInsightsConfiguration applicationInsightsConfiguration, AppMetadataConfiguration appMetadataConfiguration)
        {
            var appInsightsConfiguration = new TelemetryConfiguration(applicationInsightsConfiguration.InstrumentationKey);
            appInsightsConfiguration.TelemetryInitializers.Add(AddEnvironmentInitializer(appMetadataConfiguration));
            appInsightsConfiguration.TelemetryInitializers.Add(new TimestampAdjustmentInitializer());
            appInsightsConfiguration.TelemetryInitializers.Add(new ClientSideErrorInitializer());
            appInsightsConfiguration.TelemetryInitializers.Add(new TrackingInitializer(appMetadataConfiguration));
            appInsightsConfiguration.TelemetryInitializers.Add(new ResponseCodeTranslationIntitializer());

            var client = new TelemetryClient(appInsightsConfiguration)
            {
                InstrumentationKey = applicationInsightsConfiguration.InstrumentationKey
            };
            var wrapper = new AppInsightsTelemetryClientWrapper(client);
            return wrapper;
        }

        private EnvironmentInitializer AddEnvironmentInitializer(AppMetadataConfiguration configuration)
        {
            var environmentInitializer = new EnvironmentInitializer()
            {
                ServiceOffering = configuration.ServiceOffering,
                ServiceLine = configuration.ServiceLine,
                ComponentName = configuration.ComponentName,
                IctoId = configuration.ICTO_ID,
                EnvironmentName = configuration.Environment,
                ComponentId = configuration.ComponentId,
                Service = configuration.Service
            };
            return environmentInitializer;
        }
    }
}
