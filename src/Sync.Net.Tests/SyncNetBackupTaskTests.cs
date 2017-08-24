using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sync.Net.IO;
using Sync.Net.TestHelpers;

namespace Sync.Net.Tests
{
    [TestClass]
    public class SyncNetBackupTaskTests
    {
        private string _contents;
        private string _fileName;
        private string _fileName2;
        private string _subDirectoryName;
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
        public void CreatesTargetFile()
        {
            IDirectoryObject sourceDirectory = new MemoryDirectoryObject("directory")
                .AddFile(_fileName, _contents);

            IDirectoryObject targetDirectory = new MemoryDirectoryObject("directory");
            var syncNet = new SyncNetBackupTask(sourceDirectory, targetDirectory);
            syncNet.Run();

            var targetFile = targetDirectory.GetFile(_fileName);
            
            Assert.IsTrue(targetFile.Exists);
        }

        [TestMethod]
        public void CreatesSubDirectory()
        {
            IDirectoryObject sourceDirectory = new MemoryDirectoryObject("directory")
                .AddDirectory(_subDirectoryName);

            IDirectoryObject targetDirectory = new MemoryDirectoryObject("directory");
            var syncNet = new SyncNetBackupTask(sourceDirectory, targetDirectory);
            syncNet.Run();

            var directoryObject = targetDirectory.GetDirectory(_subDirectoryName);

            Assert.IsTrue(directoryObject.Exists);
        }

        [TestMethod]
        public void WritesFileContentToTargetFile()
        {
            IDirectoryObject sourceDirectory = new MemoryDirectoryObject("directory")
                .AddFile(_fileName, _contents);
            IDirectoryObject targetDirectory = new MemoryDirectoryObject("directory");
            var syncNet = new SyncNetBackupTask(sourceDirectory, targetDirectory);
            syncNet.Run();

            var targetFile = targetDirectory.GetFile(_fileName);
            using (var sr = new StreamReader(targetFile.GetStream()))
            {
                var targetFileContents = sr.ReadToEnd().Replace("\0", string.Empty);
                Assert.AreEqual(_contents, targetFileContents);
            }
        }

        [TestMethod]
        public void CreatesDirectoryStructure()
        {
            var sourceDirectory = new MemoryDirectoryObject("directory");

            sourceDirectory.AddDirectory(new MemoryDirectoryObject(_subDirectoryName, sourceDirectory.FullName)
                    .AddFile(_subFileName, _contents)
                    .AddFile(_subFileName2, _contents));

            IDirectoryObject targetDirectory = new MemoryDirectoryObject("directory");

            var syncNet = new SyncNetBackupTask(sourceDirectory, targetDirectory);
            syncNet.Run();

            Assert.AreEqual(0, targetDirectory.GetFiles().Count());

            var subDirectories = targetDirectory.GetDirectories();
            Assert.AreEqual(1, subDirectories.Count());
            Assert.AreEqual(_subDirectoryName, subDirectories.First().Name);

            var files = subDirectories.First().GetFiles();
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

            var syncNet = new SyncNetBackupTask(sourceDirectory, targetDirectory);
            syncNet.Run();

            var files = targetDirectory.GetFiles();
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

            var syncNet = new SyncNetBackupTask(sourceDirectory, targetDirectory);

            var fired = false;
            syncNet.ProgressChanged += delegate { fired = true; };

            syncNet.Run();

            Assert.IsTrue(fired);
        }

        [TestMethod]
        public void CountsUploadedFiles()
        {
            var sourceDirectory = new MemoryDirectoryObject("directory")
                .AddFile(_fileName, _contents)
                .AddFile(_fileName2, _contents);

            sourceDirectory.AddDirectory(new MemoryDirectoryObject(_subDirectoryName, sourceDirectory.FullName)
                    .AddFile(_subFileName, _contents)
                    .AddFile(_subFileName2, _contents));

            IDirectoryObject targetDirectory = new MemoryDirectoryObject("directory");

            var syncNet = new SyncNetBackupTask(sourceDirectory, targetDirectory);

            var progressUpdates = new List<SyncNetProgressChangedEventArgs>();
            syncNet.ProgressChanged += delegate(SyncNetBackupTask sender, SyncNetProgressChangedEventArgs e)
            {
                progressUpdates.Add(e);
            };

            syncNet.Run();

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
            var sourceDirectory = new MemoryDirectoryObject("directory")
                .AddFile(_fileName, _contents)
                .AddFile(_fileName2, _contents);

            sourceDirectory.AddDirectory(new MemoryDirectoryObject(_subDirectoryName, sourceDirectory.FullName)
                    .AddFile(_subFileName, _contents)
                    .AddFile(_subFileName2, _contents));

            IDirectoryObject targetDirectory = new MemoryDirectoryObject("directory");

            var syncNet = new SyncNetBackupTask(sourceDirectory, targetDirectory);

            var progressUpdates = new List<SyncNetProgressChangedEventArgs>();
            syncNet.ProgressChanged += delegate(SyncNetBackupTask sender, SyncNetProgressChangedEventArgs e)
            {
                progressUpdates.Add(e);
            };

            syncNet.Run();

            Assert.AreEqual(4, progressUpdates.Count);

            Assert.AreEqual(4 * bytes, progressUpdates[0].TotalBytes);
            Assert.AreEqual(bytes, progressUpdates[0].ProcessedBytes);
            Assert.AreEqual(2 * bytes, progressUpdates[1].ProcessedBytes);
            Assert.AreEqual(3 * bytes, progressUpdates[2].ProcessedBytes);
            Assert.AreEqual(4 * bytes, progressUpdates[3].ProcessedBytes);
        }

        [TestMethod]
        public void ReportsCurrentlyUploadedFileName()
        {
            var sourceDirectory = new MemoryDirectoryObject("directory")
                .AddFile(_fileName, _contents)
                .AddFile(_fileName2, _contents);

            sourceDirectory.AddDirectory(new MemoryDirectoryObject(_subDirectoryName, sourceDirectory.FullName)
                    .AddFile(_subFileName, _contents)
                    .AddFile(_subFileName2, _contents));

            IDirectoryObject targetDirectory = new MemoryDirectoryObject("directory");

            var syncNet = new SyncNetBackupTask(sourceDirectory, targetDirectory);

            var progressUpdates = new List<SyncNetProgressChangedEventArgs>();
            syncNet.ProgressChanged += delegate(SyncNetBackupTask sender, SyncNetProgressChangedEventArgs e)
            {
                progressUpdates.Add(e);
            };

            syncNet.Run();

            Assert.AreEqual(_fileName, progressUpdates[0].CurrentFile.Name);
            Assert.AreEqual(_fileName2, progressUpdates[1].CurrentFile.Name);
            Assert.AreEqual(_subFileName, progressUpdates[2].CurrentFile.Name);
            Assert.AreEqual(_subFileName2, progressUpdates[3].CurrentFile.Name);
        }

        [TestMethod]
        public void UploadsOnlyNewerFile()
        {
            var now = DateTime.Now;

            var lastUpdated = now.AddDays(-1);
            var lastUpdated2 = now.AddDays(1);

            IDirectoryObject sourceDirectory = new MemoryDirectoryObject("directory")
                .AddFile(_fileName, _contents, lastUpdated)
                .AddFile(_fileName2, _contents, lastUpdated2);

            IDirectoryObject targetDirectory = new MemoryDirectoryObject("directory")
                .AddFile(_fileName, _contents, now)
                .AddFile(_fileName2, _contents, now);

            var syncNet = new SyncNetBackupTask(sourceDirectory, targetDirectory);
            syncNet.Run();

            var files = targetDirectory.GetFiles();

            Assert.AreEqual(now, files.First().ModifiedDate);
            Assert.AreEqual(lastUpdated2, files.Last().ModifiedDate);
        }
    }
}