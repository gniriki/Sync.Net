using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Sync.Net.Configuration;
using Sync.Net.IO;

namespace Sync.Net
{
    public class SyncNetTaskFactory : ISyncNetTaskFactory
    {
        public ISyncNetTask Create(SyncNetConfiguration configuration)
        {
            return Create(configuration.LocalDirectory, configuration.S3Bucket, configuration.RegionEndpoint);
        }

        public ISyncNetTask Create(string localDirectory, string s3BucketName, RegionEndpoint regionEndpoint)
        {
            var localDirectoryObject = new LocalDirectoryObject(localDirectory);


            var amazonS3Client = new AmazonS3Client(regionEndpoint);
            var s3DirectoryObject = new S3DirectoryObject(amazonS3Client, s3BucketName);

            return new SyncNetBackupTask(localDirectoryObject, s3DirectoryObject);
        }
    }
}