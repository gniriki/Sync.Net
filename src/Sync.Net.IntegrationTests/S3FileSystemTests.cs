using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sync.Net.IO;
using Sync.Net.TestHelpers;

namespace Sync.Net.IntegrationTests
{
    [TestClass]
    public class S3FileSystemTests
    {
        private readonly string _contents = "contents.";
        private readonly string _fileName = "file.txt";
        private readonly string _fileName2 = "file2.txt";
        private readonly string _testDirectory = "backup-test1236asdsfsd11";
        private readonly string _subDirectoryName = "subDirectory";
        private readonly string _subFileName = "subFile.txt";
        private readonly string _subFileName2 = "subFile2.txt";

        [TestMethod]
        public async Task WritesFileToS3FileSystem()
        {
            var amazonS3Client = new AmazonS3Client(RegionEndpoint.USEast1);

            var directoryInfo = new S3DirectoryInfo(amazonS3Client, _testDirectory);

            if (!directoryInfo.Exists)
                directoryInfo.Create();
            else
            {
                directoryInfo.Delete(true);
            }

            var targetDirectory = new S3DirectoryObject(amazonS3Client, _testDirectory);

            var sourceDirectory = new MemoryDirectoryObject("integrationTests")
                .AddFile(_fileName, _contents)
                .AddFile(_fileName2, _contents);

            sourceDirectory.AddDirectory(new MemoryDirectoryObject(_subDirectoryName, sourceDirectory.FullName)
                    .AddFile(_subFileName, _contents)
                    .AddFile(_subFileName2, _contents));

            var sync = new SyncNetBackupTask(sourceDirectory, targetDirectory);
            await sync.ProcessSourceDirectoryAsync();

            var fileInfos = directoryInfo.GetFiles();
            Assert.AreEqual(2, fileInfos.Length);

            var directoryInfos = directoryInfo.GetDirectories();
            Assert.AreEqual(1, directoryInfos.Length);
            Assert.AreEqual(_subDirectoryName, directoryInfos[0].Name);

            var file = fileInfos[0];
            using (var sr = file.OpenText())
            {
                Assert.AreEqual(_contents, sr.ReadToEnd());
            }
        }
    }
}