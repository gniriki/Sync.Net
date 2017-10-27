using Sync.Net.Configuration;
using Sync.Net.IO;
using Sync.Net.Processing;

namespace Sync.Net
{
    public class DirectoryScannerFactory
    {
        private readonly AmazonS3ClientFactory _amazonS3ClientFactory = new AmazonS3ClientFactory();

        public IDirectoryScanner Create(ProcessorConfiguration configuration)
        {
            var localDirectoryObject = new LocalDirectoryObject(configuration.LocalDirectory);

            var amazonS3Client = _amazonS3ClientFactory.GetS3Client(configuration);
            var s3DirectoryObject = new S3DirectoryObject(amazonS3Client, configuration.S3Bucket);

            return new DirectoryScanner(localDirectoryObject, s3DirectoryObject);
        }
    }
}