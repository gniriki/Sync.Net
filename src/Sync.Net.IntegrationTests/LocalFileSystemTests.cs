using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sync.Net.IO;
using Sync.Net.TestHelpers;

namespace Sync.Net.IntegrationTests
{
    [TestClass]
    public class LocalFileSystemTests
    {
        private readonly string _contents = "contents.";
        private readonly string _fileName = "file.txt";
        private readonly string _fileName2 = "file2.txt";
        private readonly string _testDirectory = "c:\\temp\\integrationTests";
        private readonly string _subDirectoryName = "subDirectory";
        private readonly string _subFileName = "subFile.txt";
        private readonly string _subFileName2 = "subFile2.txt";

        [TestMethod]
        public void WritesFileToLocalFileSystem()
        {
            var directoryInfo = new DirectoryInfo(_testDirectory);
            if(!directoryInfo.Exists)
                directoryInfo.Create();

            var targetDirectory = new LocalDirectoryObject(directoryInfo);

            var sourceDirectory = new MemoryDirectoryObject("integrationTests")
                .AddFile(_fileName, _contents)
                .AddFile(_fileName2, _contents)
                .AddDirectory(new MemoryDirectoryObject(_subDirectoryName)
                    .AddFile(_subFileName, _contents)
                    .AddFile(_subFileName2, _contents));

            var sync = new SyncNetBackupTask(sourceDirectory, targetDirectory);
            sync.Backup();

            var fileInfos = directoryInfo.GetFiles();
            Assert.AreEqual(2, fileInfos.Length);

            var directoryInfos = directoryInfo.GetDirectories();
            Assert.AreEqual(1, directoryInfos.Length);
            Assert.AreEqual(_subDirectoryName, directoryInfos[0].Name);

            var file = fileInfos[0];
            var localContents = File.ReadAllText(file.FullName);

            Assert.AreEqual(_contents, localContents);
        }
    }
}