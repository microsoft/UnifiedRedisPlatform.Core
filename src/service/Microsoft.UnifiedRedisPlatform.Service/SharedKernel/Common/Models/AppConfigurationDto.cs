using System.Collections.Generic;

namespace Microsoft.UnifiedPlatform.Service.Common.Models
{
    public class AppConfigurationDto
    {
        public string AppName { get; set; }
        public string SupportContact { get; set; }
        public string AdminGroup { get; set; }
        public List<string> AdminApplications { get; set; }
        public string RedisCachePrefix { get; set; }
        public string WritePolicy { get; set; }
        public ConnectionPreference ConnectionPreference { get; set; }
        public DiagnosticSettingsDto DiagnosticSettings { get; set; }
    }
}
