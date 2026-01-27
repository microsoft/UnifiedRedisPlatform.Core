using System;
using Microsoft.CQRS;
using Microsoft.UnifiedPlatform.Service.Common.Models;

namespace Microsoft.UnifiedPlatform.Service.Application.Queries
{
    public class GetClusterConfigurationQuery : Query<ClusterConfigurationDto>
    {
        public override string DisplayName => "Get Cluster Configuration Query";

        private readonly string _id;
        public override string Id => _id;

        public string ClusterName { get; }
        public string AppName { get; }
        public string PreferredLocation { get; set; }
        public bool UseManagedIdentity { get; set; }

        public GetClusterConfigurationQuery(string clusterName, string appName, string preferredLocation, bool useManagedIdentity = false)
            :this(clusterName, appName)
        {
            PreferredLocation = preferredLocation;
            UseManagedIdentity = useManagedIdentity;
        }

        public GetClusterConfigurationQuery(string clusterName, string appName)
        {
            _id = Guid.NewGuid().ToString();
            ClusterName = clusterName;
            AppName = appName;
        }

        public override bool Validate(out string ValidationErrorMessage)
        {
            ValidationErrorMessage = null;

            if (string.IsNullOrWhiteSpace(ClusterName))
                ValidationErrorMessage = "Cluster Name cannot be empty";

            if (string.IsNullOrWhiteSpace(AppName))
                ValidationErrorMessage = "App Name cannot be empty";

            return string.IsNullOrWhiteSpace(ValidationErrorMessage);
        }
    }
}
