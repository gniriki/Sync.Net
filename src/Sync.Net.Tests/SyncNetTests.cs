using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sync.Net.Tests
{
    [TestClass]
    public class SyncNetTests
    {
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
        }

        [TestMethod]
        public void CreatesFileInTargetDirectory()
        {
            var targetDirectory = new MemoryDirectoryObject("directory");

            SyncNet syncNet = new SyncNet(new MemoryFileObject("file.txt"), targetDirectory);
            syncNet.Backup();
            Assert.IsTrue(targetDirectory.ContainsFile("file.txt"));
        }

        [TestMethod]
        public void WritesFileContentToTargetFile()
        {
            IDirectoryObject memoryDirectoryObject = new MemoryDirectoryObject("directory");
            SyncNet syncNet = new SyncNet(new MemoryFileObject(_fileName, _contents), memoryDirectoryObject);
            syncNet.Backup();

            IFileObject targetFile = memoryDirectoryObject.GetFile(_fileName);
            using (StreamReader sr = new StreamReader(targetFile.GetStream()))
            {
                string targetFileContents = sr.ReadToEnd().Replace("\0", string.Empty);
                Assert.AreEqual(_contents, targetFileContents);
            }
        }

        [TestMethod]
        public void CreatesDirectoryStructure()
        {
            IDirectoryObject sourceDirectory = new MemoryDirectoryObject("directory")
                .AddDirectory(new MemoryDirectoryObject(_subDirectoryName)
                    .AddFile(_subFileName, _contents)
                    .AddFile(_subFileName2, _contents));

            IDirectoryObject targetDirectory = new MemoryDirectoryObject("directory");

            SyncNet syncNet = new SyncNet(sourceDirectory, targetDirectory);
            syncNet.Backup();

            Assert.AreEqual(0, targetDirectory.GetFiles().Count());

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
            IDirectoryObject sourceDirectory = new MemoryDirectoryObject("directory")
                .AddFile(_fileName, _contents)
                .AddFile(_fileName2, _contents);

            IDirectoryObject targetDirectory = new MemoryDirectoryObject("directory");

            SyncNet syncNet = new SyncNet(sourceDirectory, targetDirectory);
            syncNet.Backup();

            IEnumerable<IFileObject> files = targetDirectory.GetFiles();
            Assert.AreEqual(2, files.Count());

            Assert.IsTrue(files.Any(x => x.Name == _fileName));
            Assert.IsTrue(files.Any(x => x.Name == _fileName2));
        }

        [TestMethod]
        public void FiresProgressEvent()
        {
            IDirectoryObject sourceDirectory = new MemoryDirectoryObject("directory")
                .AddFile(_fileName, _contents);

            IDirectoryObject targetDirectory = new MemoryDirectoryObject("directory");

            SyncNet syncNet = new SyncNet(sourceDirectory, targetDirectory);

            var fired = false;
            syncNet.ProgressChanged += delegate
            {
                fired = true;
            };

            syncNet.Backup();

            Assert.IsTrue(fired);
        }

        [TestMethod]
        public void CountsUploadedFiles()
        {
            IDirectoryObject sourceDirectory = new MemoryDirectoryObject("directory")
                .AddFile(_fileName, _contents)
                .AddFile(_fileName2, _contents)
                .AddDirectory(new MemoryDirectoryObject(_subDirectoryName)
                    .AddFile(_subFileName, _contents)
                    .AddFile(_subFileName2, _contents));

            IDirectoryObject targetDirectory = new MemoryDirectoryObject("directory");

            SyncNet syncNet = new SyncNet(sourceDirectory, targetDirectory);

            var progressUpdates = new List<SyncNetProgressChangedEventArgs>();
            syncNet.ProgressChanged += delegate (SyncNet sender, SyncNetProgressChangedEventArgs e)
            {
                progressUpdates.Add(e);
            };

            syncNet.Backup();

            Assert.AreEqual(4, progressUpdates.Count);

            Assert.AreEqual(4, progressUpdates[0].TotalFiles);
            Assert.AreEqual(1, progressUpdates[0].ProcessedFiles);
            Assert.AreEqual(2, progressUpdates[1].ProcessedFiles);
            Assert.AreEqual(3, progressUpdates[2].ProcessedFiles);
            Assert.AreEqual(4, progressUpdates[3].ProcessedFiles);
        }

        [TestMethod]
        public void CountsUploadedData()
        {
            var bytes = Encoding.UTF8.GetBytes(_contents).Length;
            IDirectoryObject sourceDirectory = new MemoryDirectoryObject("directory")
                .AddFile(_fileName, _contents)
                .AddFile(_fileName2, _contents)
                .AddDirectory(new MemoryDirectoryObject(_subDirectoryName)
                    .AddFile(_subFileName, _contents)
                    .AddFile(_subFileName2, _contents));

            IDirectoryObject targetDirectory = new MemoryDirectoryObject("directory");

            SyncNet syncNet = new SyncNet(sourceDirectory, targetDirectory);

            var progressUpdates = new List<SyncNetProgressChangedEventArgs>();
            syncNet.ProgressChanged += delegate (SyncNet sender, SyncNetProgressChangedEventArgs e)
            {
                progressUpdates.Add(e);
            };

            syncNet.Backup();

            Assert.AreEqual(4, progressUpdates.Count);

            Assert.AreEqual(4* bytes, progressUpdates[0].TotalBytes);
            Assert.AreEqual(bytes, progressUpdates[0].ProcessedBytes);
            Assert.AreEqual(2 * bytes, progressUpdates[1].ProcessedBytes);
            Assert.AreEqual(3 * bytes, progressUpdates[2].ProcessedBytes);
            Assert.AreEqual(4 * bytes, progressUpdates[3].ProcessedBytes);
        }
    }

   
}
