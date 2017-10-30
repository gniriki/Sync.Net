using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sync.Net.IO;
using Sync.Net.Processing;
using Sync.Net.TestHelpers;

namespace Sync.Net.IntegrationTests
{
    [TestClass]
    public class S3FileSystemTests
    {
        private readonly string _testDirectory = "backup-test1236asdsfsd11";
        private S3DirectoryInfo _s3DirectoryInfo;
        private S3DirectoryObject _targetDirectory;
        private MemoryDirectoryObject _sourceObject;

        [TestInitialize]
        public void Init()
        {
            var amazonS3Client = new AmazonS3Client(RegionEndpoint.USEast1);

            _s3DirectoryInfo = new S3DirectoryInfo(amazonS3Client, _testDirectory);

            if (_s3DirectoryInfo.Exists)
                _s3DirectoryInfo.Delete(true);
            _s3DirectoryInfo.Create();

            _targetDirectory = new S3DirectoryObject(amazonS3Client, _testDirectory);

            _sourceObject = DirectoryHelper.CreateFullDirectory();
        }

        [TestMethod]
        public void WritesFileToS3FileSystem()
        {
            var syncNet = new Processor(_sourceObject, _targetDirectory, new SyncTaskQueue());
            syncNet.ProcessSourceDirectory();

            var fileInfos = _s3DirectoryInfo.GetFiles();
            Assert.AreEqual(2, fileInfos.Length);

            var directoryInfos = _s3DirectoryInfo.GetDirectories();
            Assert.AreEqual(1, directoryInfos.Length);
            Assert.AreEqual(DirectoryHelper.SubDirectoryName, directoryInfos[0].Name);

            var file = fileInfos[0];
            using (var sr = file.OpenText())
            {
                Assert.AreEqual(DirectoryHelper.Contents, sr.ReadToEnd());
            }
        }

        [TestMethod]
        public void RenameFileOnS3FileSystem()
        {
            var syncNet = new Processor(_sourceObject, _targetDirectory, new SyncTaskQueue());
            syncNet.ProcessSourceDirectory();

            var fileToRename = _sourceObject.GetDirectories().First().GetFiles().First();
            syncNet.RenameFile(fileToRename, "newName.txt");

            var fileInfos = _s3DirectoryInfo.GetDirectories().First().GetFiles();
            Assert.IsTrue(fileInfos.Any(x => x.Name == "newName.txt"));
            Assert.IsFalse(fileInfos.Any(x => x.Name == fileToRename.Name));
        }
    }
}