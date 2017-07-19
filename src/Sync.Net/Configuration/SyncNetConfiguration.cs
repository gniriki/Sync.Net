using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;

namespace Sync.Net.Configuration
{
    public class SyncNetConfiguration
    {
        public string LocalDirectory { get; set; }
        public string S3Bucket { get; set; }
        public RegionEndpoint RegionEndpoint { get; set; }
    }
}
