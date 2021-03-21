namespace Microsoft.UnifiedPlatform.Service.Common.Configuration.Resolvers
{
    public class ApplicationInsightsConfigurationResolver : IConfigurationResolver<ApplicationInsightsConfiguration>
    {
        private readonly BaseConfigurationProvider _configurationProvider;

        /// <summary>
        /// Constructs the Resolved
        /// </summary>
        /// <param name="configurationProvider" cref="BaseConfigurationProvider">Resolves the configuration value from a source</param>
        public ApplicationInsightsConfigurationResolver(BaseConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        /// <summary>
        /// Creates and resolves the Application Insights configuration
        /// </summary>
        /// <returns  cref="ApplicationInsightsConfiguration">Application Insights Configuration</returns>
        public ApplicationInsightsConfiguration Resolve()
        {
            return new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = _configurationProvider.GetConfiguration("ApplicationInsights", "InstrumentationKey").Result,
                LogLevel = (TraceLevel)(int.Parse(_configurationProvider.GetConfiguration("ApplicationInsights", "TraceLevel").Result)),
            };
        }
    }
}
