using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Schema;
using Amazon;

namespace Sync.Net.Configuration
{
    public class ProcessorConfigurationValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
    }

    [DataContract]
    public class ProcessorConfiguration
    {
        public ProcessorConfiguration()
        {
            RegionEndpoint = RegionEndpoint.USEast1;
        }

        [DataMember]
        public string RegionEndpointSystemName { get; private set; }

        [DataMember]
        public string LocalDirectory { get; set; }

        [DataMember]
        public string S3Bucket { get; set; }

        public RegionEndpoint RegionEndpoint
        {
            get { return RegionEndpoint.GetBySystemName(RegionEndpointSystemName); }
            set { RegionEndpointSystemName = value.SystemName; }
        }

        [DataMember]
        public CredentialsType? CredentialsType { get; set; }

        [DataMember]
        public string KeyId { get; set; }

        [DataMember]
        public string KeySecret { get; set; }

        [DataMember]
        public string ProfileName { get; set; }

        public ProcessorConfigurationValidationResult Validate()
        {
            if (!CredentialsType.HasValue)
                return Invalid("Choose credentials");
            switch (CredentialsType.Value)
            {
                case Configuration.CredentialsType.DefaultProfile:
                    break;
                case Configuration.CredentialsType.NamedProfile:
                    if (string.IsNullOrEmpty(ProfileName))
                        return Invalid("Profile name cannot be empty");
                    break;
                case Configuration.CredentialsType.Basic:
                    if (string.IsNullOrEmpty(KeyId))
                        return Invalid("KeyId cannot be empty");
                    if (string.IsNullOrEmpty(KeySecret))
                        return Invalid("KeySecret cannot be empty");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Valid();
        }

        private ProcessorConfigurationValidationResult Valid()
        {
            return new ProcessorConfigurationValidationResult { IsValid = true };
        }

        private ProcessorConfigurationValidationResult Invalid(string message)
        {
            return new ProcessorConfigurationValidationResult { IsValid = false, Message = message};
        }
    }
}