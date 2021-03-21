namespace Microsoft.UnifiedPlatform.Service.Common.Models
{
    public class ConnectionStringDto
    {
        public RegionDto Region { get; set; }
        public string ConnectionStringLocation { get; set; }
        public string ConnectionString { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsWriteEnabled { get; set; }
    }
}
