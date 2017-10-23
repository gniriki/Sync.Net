using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3.Model;

namespace Sync.Net.Configuration
{
    public class ConfigurationTestResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
    }

    public class ConfigurationTester : IConfigurationTester
    {
        private string _key = "key";
        private string _contentBody = "content";

        public ConfigurationTestResult Test(SyncNetConfiguration configuration)
        {
            AmazonS3ClientFactory factory = new AmazonS3ClientFactory();
            var client = factory.GetS3Client(configuration);

            try
            {
                client.ListObjects(configuration.S3Bucket);

                client.PutObject(new PutObjectRequest
                {
                    BucketName = configuration.S3Bucket,
                    ContentBody = _contentBody,
                    Key = _key
                });

                client.DeleteObject(configuration.S3Bucket, _key);
            }
            catch (Exception e)
            {
                return new ConfigurationTestResult {IsValid = false, Message = e.Message};
            }

            return new ConfigurationTestResult { IsValid = true };
        }
    }
}
