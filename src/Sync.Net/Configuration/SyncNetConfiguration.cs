using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Schema;
using Amazon;

namespace Sync.Net.Configuration
{
    [DataContract]
    public class SyncNetConfiguration
    {
        private static readonly DataContractSerializer Serializer;

        static SyncNetConfiguration()
        {
            Serializer = new DataContractSerializer(typeof(SyncNetConfiguration));
        }

        public SyncNetConfiguration()
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
        public CredentialsType CredentialsType { get; set; }

        [DataMember]
        public string KeyId { get; set; }

        [DataMember]
        public string KeySecret { get; set; }

        [DataMember]
        public string ProfileName { get; set; }

        public virtual void Save(Stream stream)
        {
            Validate();
            Serializer.WriteObject(stream, this);
        }

        private void Validate()
        {
            switch (CredentialsType)
            {
                case CredentialsType.DefaultProfile:
                    break;
                case CredentialsType.NamedProfile:
                    if(string.IsNullOrEmpty(ProfileName))
                        throw new ConfigurationException("Profile name cannot be empty");
                    break;
                case CredentialsType.Basic:
                    if (string.IsNullOrEmpty(KeyId))
                        throw new ConfigurationException("KeyId cannot be empty");
                    if (string.IsNullOrEmpty(KeySecret))
                        throw new ConfigurationException("KeySecret cannot be empty");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static SyncNetConfiguration Load(Stream stream)
        {
            if (stream.Length == 0)
                throw new ConfigurationLoadException("The configuration file is empty");

            try
            {
                return Serializer.ReadObject(stream) as SyncNetConfiguration;
            }
            catch (Exception e)
            {
                throw new ConfigurationLoadException("Error while loading the configuration from file", e);
            }
        }
    }
}