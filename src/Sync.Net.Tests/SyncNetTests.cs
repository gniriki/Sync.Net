using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sync.Net.Tests
{
    [TestClass]
    public class SyncNetTests
    {
        private IDirectoryObject _sourceDirectory;
        private string _fileName;
        private string _fileName2;
        private string _subDirectoryName;
        private string _contents;
        private string _subFileName;
        private string _subFileName2;

        [TestInitialize]
        public void Initialize()
        {
            _fileName = "file.txt";
            _fileName2 = "file2.txt";
            _subFileName = "subFile.txt";
            _subFileName2 = "subfile2.txt";
            _subDirectoryName = "dir";
            _contents = "This is file content";

            _sourceDirectory = new MemoryDirectoryObject("directory")
                .AddFile(_fileName, _contents)
                .AddFile(_fileName2, _contents)
                .AddDirectory(new MemoryDirectoryObject(_subDirectoryName)
                    .AddFile(_subFileName, _contents)
                    .AddFile(_subFileName2, _contents));
        }

        [TestMethod]
        public void CreatesFileInTargetDirectory()
        {
            SyncNet syncNet = new SyncNet();
            var targetDirectory = new MemoryDirectoryObject("directory");

            syncNet.Backup(new MemoryFileObject("file.txt"), targetDirectory);
            Assert.IsTrue(targetDirectory.ContainsFile("file.txt"));
        }

        [TestMethod]
        public void WritesFileContentToTargetFile()
        {
            SyncNet syncNet = new SyncNet();
            IDirectoryObject memoryDirectoryObject = new MemoryDirectoryObject("directory");

            var fileName = "file.txt";
            var contents = "This is file content";

            syncNet.Backup(new MemoryFileObject(fileName, contents), memoryDirectoryObject);

            IFileObject targetFile = memoryDirectoryObject.GetFile(fileName);
            using (StreamReader sr = new StreamReader(targetFile.GetStream()))
            {
                string targetFileContents = sr.ReadToEnd().Replace("\0", string.Empty);
                Assert.AreEqual(contents, targetFileContents);
            }
        }

        [TestMethod]
        public void CreatesDirectoryStructure()
        {
            IDirectoryObject targetDirectory = new MemoryDirectoryObject("directory");

            SyncNet syncNet = new SyncNet();
            syncNet.Backup(_sourceDirectory, targetDirectory);

            IEnumerable<IDirectoryObject> subDirectories = targetDirectory.GetDirectories();
            Assert.AreEqual(1, subDirectories.Count());
            Assert.AreEqual(_subDirectoryName, subDirectories.First().Name);

            IEnumerable<IFileObject> files = subDirectories.First().GetFiles();
            Assert.AreEqual(2, files.Count());

            Assert.IsTrue(files.Any(x => x.Name == _subFileName));
            Assert.IsTrue(files.Any(x => x.Name == _subFileName2));
        }

        [TestMethod]
        public void UploadsFilesToDirectory()
        {
            IDirectoryObject targetDirectory = new MemoryDirectoryObject("directory");

            SyncNet syncNet = new SyncNet();
            syncNet.Backup(_sourceDirectory, targetDirectory);

            IEnumerable<IFileObject> files = targetDirectory.GetFiles();
            Assert.AreEqual(2, files.Count());

            Assert.IsTrue(files.Any(x => x.Name == _fileName));
            Assert.IsTrue(files.Any(x => x.Name == _fileName2));
        }

        [TestMethod]
        public void BackupUpdatesProgress()
        {

        }
    }
}
