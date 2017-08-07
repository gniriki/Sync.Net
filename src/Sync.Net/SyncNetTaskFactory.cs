using Amazon;
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

        public ISyncNetTask Create(string localDirectory, string s3Bucket, RegionEndpoint regionEndpoint)
        {
            var localDirectoryObject = new LocalDirectoryObject(localDirectory);
            var s3DirectoryObject = new S3DirectoryObject(new AmazonS3Client(regionEndpoint), s3Bucket);

            return new SyncNetBackupTask(localDirectoryObject, s3DirectoryObject);
        }
    }
}