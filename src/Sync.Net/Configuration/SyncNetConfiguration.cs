using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Amazon;
using Sync.Net.IO;

namespace Sync.Net.Configuration
{
    [DataContract]
    public class SyncNetConfiguration
    {
        private static readonly DataContractSerializer Serializer;

        [DataMember]
        public string RegionEndpointSystemName { get; private set; }

        public SyncNetConfiguration()
        {
            RegionEndpoint = RegionEndpoint.USEast1;
        }

        static SyncNetConfiguration()
        {
            Serializer = new DataContractSerializer(typeof(SyncNetConfiguration));
        }

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
