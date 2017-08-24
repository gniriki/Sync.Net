using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task WritesFileToLocalFileSystem()
        {
            var directoryInfo = new DirectoryInfo(_testDirectory);
            if (!directoryInfo.Exists)
                directoryInfo.Create();
            else
            {
                directoryInfo.Delete(true);
            }

            var targetDirectory = new LocalDirectoryObject(directoryInfo);

            var sourceDirectory = new MemoryDirectoryObject("integrationTests")
                .AddFile(_fileName, _contents)
                .AddFile(_fileName2, _contents);

            sourceDirectory.AddDirectory(new MemoryDirectoryObject(_subDirectoryName, sourceDirectory.FullName)
                    .AddFile(_subFileName, _contents)
                    .AddFile(_subFileName2, _contents));

            var sync = new SyncNetBackupTask(sourceDirectory, targetDirectory);
            await sync.ProcessFilesAsync();

            var fileInfos = directoryInfo.GetFiles();
            Assert.AreEqual(2, fileInfos.Length);

            var directoryInfos = directoryInfo.GetDirectories();
            Assert.AreEqual(1, directoryInfos.Length);
            Assert.AreEqual(_subDirectoryName, directoryInfos[0].Name);

            var file = fileInfos[0];
            var localContents = File.ReadAllText(file.FullName);

            Assert.AreEqual(_contents, localContents);
        }

        [TestMethod]
        public async Task UploadsFile()
        {
            File.WriteAllText(Path.Combine(_testDirectory, _subDirectoryName, _subFileName), 
                _contents);

            var sourceDirectory = new LocalDirectoryObject(_testDirectory);

            var targetDirectory = new MemoryDirectoryObject("dir");

            var sync = new SyncNetBackupTask(sourceDirectory, targetDirectory);

            await sync.ProcessFileAsync(sourceDirectory.FullName + "\\" + _subDirectoryName + "\\" + _subFileName);

            var subDirectory = targetDirectory.GetDirectories().First();

            Assert.AreEqual(_subDirectoryName, subDirectory.Name);

            var fileObject = subDirectory.GetFiles().First();

            Assert.AreEqual(_subFileName, fileObject.Name);
            using(var sr = new StreamReader(fileObject.GetStream()))
                Assert.AreEqual(_contents, sr.ReadToEnd().Replace("\0", String.Empty));
        }
    }
}