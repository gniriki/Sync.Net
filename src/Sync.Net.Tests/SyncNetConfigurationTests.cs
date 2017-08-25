using System.IO;
using Amazon;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sync.Net.Configuration;

namespace Sync.Net.Tests
{
    [TestClass]
    public class SyncNetConfigurationTests
    {
        private readonly string _configLocalDirectory = "testLocal";
        private readonly RegionEndpoint _configRegionEndpoint = RegionEndpoint.APNortheast1;
        private readonly string _configS3Bucket = "testBucket";

        [TestMethod]
        public void SaveLoadTest()
        {
            var config = new SyncNetConfiguration
            {
                LocalDirectory = _configLocalDirectory,
                S3Bucket = _configS3Bucket,
                RegionEndpoint = _configRegionEndpoint
            };

            var memoryStream = new MemoryStream();
            config.Save(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            var loadedConfiguration1 = SyncNetConfiguration.Load(memoryStream);
            var loadedConfiguration = loadedConfiguration1;

            Assert.AreEqual(_configLocalDirectory, loadedConfiguration.LocalDirectory);
            Assert.AreEqual(_configS3Bucket, loadedConfiguration.S3Bucket);
            Assert.AreEqual(_configRegionEndpoint, loadedConfiguration.RegionEndpoint);
        }
    }
}