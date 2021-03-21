namespace Microsoft.UnifiedPlatform.Service.Common.Configuration
{
    public class AppMetadataConfiguration : BaseConfiguration
    {
        public override string Name => "App Metadata Configuration";

        public string Environment { get; set; }
        public string ServiceOffering { get; set; }
        public string ServiceLine { get; set; }
        public string Service { get; set; }
        public string Capability { get; set; }
        public string ComponentName { get; set; }
        public string ComponentId { get; set; }
        public string ICTO_ID { get; set; }
        public string TenantNameHeaderKey { get; set; }
        public string CorrelationIdHeaderKey { get; set; }
        public string TransactionIdHeaderKey { get; set; }
        public string SecretHeaderKey { get; set; }
        public string PreferredLocationHeaderKey { get; set; }
        public string ClusterClaimsKey { get; set; }
        public string AppClaimsKey { get; set; }
        public string AdministratorGroup { get; set; }
        public RegionConfiguration Region { get; set; }
    }

    public class RegionConfiguration
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
