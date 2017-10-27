﻿using System;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Sync.Net.Configuration;
using Sync.Net.IO;

namespace Sync.Net
{
    public class ProcessorFactory : IProcessorFactory
    {
        private readonly AmazonS3ClientFactory _amazonS3ClientFactory = new AmazonS3ClientFactory();

        public IProcessor Create(ProcessorConfiguration configuration)
        {
            var localDirectoryObject = new LocalDirectoryObject(configuration.LocalDirectory);

            var amazonS3Client = _amazonS3ClientFactory.GetS3Client(configuration);
            var s3DirectoryObject = new S3DirectoryObject(amazonS3Client, configuration.S3Bucket);

            return new Processor(localDirectoryObject, s3DirectoryObject);
        }
    }
}