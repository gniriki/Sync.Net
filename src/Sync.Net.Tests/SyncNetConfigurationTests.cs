using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sync.Net.Configuration;
using Sync.Net.TestHelpers;

namespace Sync.Net.Tests
{
    [TestClass]
    public class SyncNetConfigurationTests
    {
        [TestMethod]
        public void SaveLoadTest()
        {
            var configLocalDirectory = "testLocal";
            var configS3Bucket = "testBucket";
            var configRegionEndpoint = RegionEndpoint.APNortheast1;

            var config = new SyncNetConfiguration
            {
                LocalDirectory = configLocalDirectory,
                S3Bucket = configS3Bucket,
                RegionEndpoint = configRegionEndpoint
            };


            var memoryStream = new MemoryStream();
            config.Save(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            var loadedConfiguration = SyncNetConfiguration.Load(memoryStream);
            Assert.AreEqual(configLocalDirectory, loadedConfiguration.LocalDirectory);
            Assert.AreEqual(configS3Bucket, loadedConfiguration.S3Bucket);
            Assert.AreEqual(configRegionEndpoint, loadedConfiguration.RegionEndpoint);
        }
    }
}
