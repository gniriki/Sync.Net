using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sync.Net.IO;
using Sync.Net.Processing;
using Sync.Net.TestHelpers;

namespace Sync.Net.Tests
{
    [TestClass]
    public class ProcessorTests
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
            var syncNet = new Processor(sourceDirectory, targetDirectory, new SyncTaskQueue());
            syncNet.ProcessSourceDirectory();

            var targetFile = targetDirectory.GetFile(_fileName);

            Assert.IsTrue(targetFile.Exists);
        }

        [TestMethod]
        public void CreatesSubDirectory()
        {
            var sourceDirectory = new MemoryDirectoryObject("directory")
                .AddDirectory(_subDirectoryName);

            IDirectoryObject targetDirectory = new MemoryDirectoryObject("directory");
            var syncNet = new Processor(sourceDirectory, targetDirectory, new SyncTaskQueue());
            syncNet.ProcessSourceDirectory();

            var directoryObject = targetDirectory.GetDirectory(_subDirectoryName);

            Assert.IsTrue(directoryObject.Exists);
        }

        [TestMethod]
        public void WritesFileContentToTargetFile()
        {
            IDirectoryObject sourceDirectory = new MemoryDirectoryObject("directory")
                .AddFile(_fileName, _contents);
            IDirectoryObject targetDirectory = new MemoryDirectoryObject("directory");
            var syncNet = new Processor(sourceDirectory, targetDirectory, new SyncTaskQueue());
            syncNet.ProcessSourceDirectory();

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

            var syncNet = new Processor(sourceDirectory, targetDirectory, new SyncTaskQueue());
            syncNet.ProcessSourceDirectory();

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

            var syncNet = new Processor(sourceDirectory, targetDirectory, new SyncTaskQueue());
            syncNet.ProcessSourceDirectory();

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

            var syncNet = new Processor(sourceDirectory, targetDirectory, new SyncTaskQueue());


            var fired = false;
            syncNet.ProgressChanged += delegate { fired = true; };
            syncNet.ProcessSourceDirectory();

            Assert.IsTrue(fired);
        }

        [TestMethod]
        public void CountsFilesLeft()
        {
            var sourceDirectory = new MemoryDirectoryObject("directory")
                .AddFile(_fileName, _contents)
                .AddFile(_fileName2, _contents);

            sourceDirectory.AddDirectory(new MemoryDirectoryObject(_subDirectoryName, sourceDirectory.FullName)
                .AddFile(_subFileName, _contents)
                .AddFile(_subFileName2, _contents));

            IDirectoryObject targetDirectory = new MemoryDirectoryObject("directory");

            var queue = new ManualTaskQueue();
            var syncNet = new Processor(sourceDirectory, targetDirectory, queue);


            var progressUpdates = new List<SyncNetProgressChangedEventArgs>();
            syncNet.ProgressChanged += delegate (Processor sender, SyncNetProgressChangedEventArgs e)
            {
                progressUpdates.Add(e);
            };

            syncNet.ProcessSourceDirectory();

            queue.ExecuteAll();
            Assert.AreEqual(3, progressUpdates[0].FilesLeft);
            Assert.AreEqual(2, progressUpdates[1].FilesLeft);
            Assert.AreEqual(1, progressUpdates[2].FilesLeft);
            Assert.AreEqual(0, progressUpdates[3].FilesLeft);
        }
    }
}