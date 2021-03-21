using System;
using Microsoft.CQRS;

namespace Microsoft.UnifiedRedisPlatform.Manager.Domain.Commands
{
    public class CreateKeyCommand : Command<IdCommandResult>
    {
        public override string DisplayName => "Create Key Command";

        private readonly string _id;
        public override string Id => _id;

        public string Cluster { get; set; }
        public string Application { get; set; }
        public string Key { get; set; }
        public string Value{ get; set; }
        public string DataType { get; set; }

        public CreateKeyCommand(string cluster, string application, string key, string value)
        {
            _id = Guid.NewGuid().ToString();
            Cluster = cluster;
            Application = application;
            Key = key;
            Value = value;
        }

        public override bool Validate(out string ValidationErrorMessage)
        {
            ValidationErrorMessage = null;
            
            if (string.IsNullOrWhiteSpace(Cluster))
                ValidationErrorMessage += "Cluster name cannot be empty.";
            if (string.IsNullOrWhiteSpace(Application))
                ValidationErrorMessage += "Application name cannot be empty.";
            if (string.IsNullOrWhiteSpace(Key))
                ValidationErrorMessage += "Key cannot be empty";

            return string.IsNullOrWhiteSpace(ValidationErrorMessage);
        }
    }
}
