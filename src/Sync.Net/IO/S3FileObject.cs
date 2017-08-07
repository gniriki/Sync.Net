using System;
using System.IO;
using Amazon.S3;
using Amazon.S3.IO;

namespace Sync.Net.IO
{
    public class S3FileObject : IFileObject
    {
        private readonly S3FileInfo _s3FileInfo;

        public S3FileObject(S3FileInfo s3FileInfo)
        {
            _s3FileInfo = s3FileInfo;
        }

        public S3FileObject(IAmazonS3 s3Client, string bucketName, string key)
            : this(new S3FileInfo(s3Client, bucketName, key))
        {
        }

        public string Name => _s3FileInfo.Name;
        public bool Exists => _s3FileInfo.Exists;
        public long Size => _s3FileInfo.Length;

        public DateTime ModifiedDate
        {
            get { return _s3FileInfo.LastWriteTime; }
            set { }
        }

        public Stream GetStream()
        {
            return _s3FileInfo.OpenWrite();
        }

        public void Create()
        {
            using (var stream = _s3FileInfo.OpenWrite())
            {
                ;
            }
        }
    }
}