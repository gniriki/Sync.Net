﻿using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.S3;
using Amazon.S3.IO;

namespace Sync.Net.IO
{
    public class S3DirectoryObject : IDirectoryObject
    {
        private readonly string _bucketName;
        private readonly IAmazonS3 _s3Client;
        private readonly S3DirectoryInfo _s3DirectoryInfo;
        private string key;

        public S3DirectoryObject(string bucketName)
            : this(new AmazonS3Client(), bucketName)
        {
        }

        public S3DirectoryObject(IAmazonS3 s3Client, string bucketName)
            : this(s3Client, new S3DirectoryInfo(s3Client, bucketName))
        {
            _bucketName = bucketName;
        }

        private S3DirectoryObject(IAmazonS3 s3Client, S3DirectoryInfo directoryInfo)
        {
            _s3DirectoryInfo = directoryInfo;
            _bucketName = _s3DirectoryInfo.Bucket.Name;
            _s3Client = s3Client;
        }

        public S3DirectoryObject(IAmazonS3 s3Client, string bucketName, string key) :
            this(s3Client, new S3DirectoryInfo(s3Client, bucketName, key))
        {
        }

        public string Name => _s3DirectoryInfo.Name;
        public bool Exists => _s3DirectoryInfo.Exists;
        public string FullName => _s3DirectoryInfo.FullName;

        public IFileObject GetFile(string name)
        {
            return new S3FileObject(_s3Client, _bucketName, GetSubKey(name));
        }

        public IEnumerable<IFileObject> GetFiles(bool recursive = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDirectoryObject> GetDirectories()
        {
            return _s3DirectoryInfo.GetDirectories().Select(x => new S3DirectoryObject(_s3Client, x));
        }

        public void Create()
        {
            _s3DirectoryInfo.Create();
        }

        public IDirectoryObject GetDirectory(string name)
        {
            return new S3DirectoryObject(_s3Client,
                _bucketName,
                GetSubKey(name));
        }

        private string GetSubKey(string key)
        {
            if (_s3DirectoryInfo.Bucket.Name == _s3DirectoryInfo.Name)
                return key;
            return _s3DirectoryInfo.Name + $"\\{key}";
        }
    }
}