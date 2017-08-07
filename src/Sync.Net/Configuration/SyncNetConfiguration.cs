using System.IO;
using System.Runtime.Serialization;
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

        public virtual void Save(Stream stream)
        {
            Serializer.WriteObject(stream, this);
        }

        public static SyncNetConfiguration Load(Stream stream)
        {
            return Serializer.ReadObject(stream) as SyncNetConfiguration;
        }
    }
}