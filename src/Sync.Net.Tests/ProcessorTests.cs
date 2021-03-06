﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sync.Net.IO;
using Sync.Net.Processing;
using Sync.Net.TestHelpers;

namespace Sync.Net.Tests
{
    [TestClass]
    public class ProcessorTests
    {
        [TestMethod]
        public void CreatesTargetFile()
        {
            IDirectoryObject sourceDirectory = DirectoryHelper.CreateDirectoryWithFile();

            IDirectoryObject targetDirectory = new MemoryDirectoryObject("directory");
            var syncNet = new Processor(sourceDirectory, targetDirectory, new SyncTaskQueue());
            syncNet.ProcessSourceDirectory();

            var targetFile = targetDirectory.GetFile(DirectoryHelper.FileName);

            Assert.IsTrue(targetFile.Exists);
        }

        [TestMethod]
        public void CreatesSubDirectory()
        {
            var sourceDirectory = new MemoryDirectoryObject("directory")
                .AddDirectory(DirectoryHelper.SubDirectoryName);

            IDirectoryObject targetDirectory = new MemoryDirectoryObject("directory");
            var syncNet = new Processor(sourceDirectory, targetDirectory, new SyncTaskQueue());
            syncNet.ProcessSourceDirectory();

            var directoryObject = targetDirectory.GetDirectory(DirectoryHelper.SubDirectoryName);

            Assert.IsTrue(directoryObject.Exists);
        }

        [TestMethod]
        public void WritesFileContentToTargetFile()
        {
            IDirectoryObject sourceDirectory = DirectoryHelper.CreateDirectoryWithFile();

            IDirectoryObject targetDirectory = new MemoryDirectoryObject("directory");
            var syncNet = new Processor(sourceDirectory, targetDirectory, new SyncTaskQueue());
            syncNet.ProcessSourceDirectory();

            var targetFile = targetDirectory.GetFile(DirectoryHelper.FileName);
            using (var sr = new StreamReader(targetFile.GetStream()))
            {
                var targetFileContents = sr.ReadToEnd().Replace("\0", string.Empty);
                Assert.AreEqual(DirectoryHelper.Contents, targetFileContents);
            }
        }

        [TestMethod]
        public void CreatesDirectoryStructure()
        {
            var sourceDirectory = DirectoryHelper.CreateFullDirectory();

            IDirectoryObject targetDirectory = new MemoryDirectoryObject("directory");

            var syncNet = new Processor(sourceDirectory, targetDirectory, new SyncTaskQueue());
            syncNet.ProcessSourceDirectory();

            Assert.AreEqual(2, targetDirectory.GetFiles().Count());

            var subDirectories = targetDirectory.GetDirectories();
            Assert.AreEqual(1, subDirectories.Count());
            Assert.AreEqual(DirectoryHelper.SubDirectoryName, subDirectories.First().Name);

            var files = subDirectories.First().GetFiles();
            Assert.AreEqual(2, files.Count());

            Assert.IsTrue(files.Any(x => x.Name == DirectoryHelper.SubFileName));
            Assert.IsTrue(files.Any(x => x.Name == DirectoryHelper.SubFileName2));
        }

        [TestMethod]
        public void FiresProgressEvent()
        {
            IDirectoryObject sourceDirectory = DirectoryHelper.CreateDirectoryWithFile();

            IDirectoryObject targetDirectory = new MemoryDirectoryObject("directory");

            var syncNet = new Processor(sourceDirectory, targetDirectory, new SyncTaskQueue());


            var fired = false;
            syncNet.ProgressChanged += delegate { fired = true; };
            syncNet.ProcessSourceDirectory();

            Assert.IsTrue(fired);
        }

        [TestMethod]
        public void RenamesFile()
        {
            IDirectoryObject sourceDirectory = DirectoryHelper.CreateDirectoryWithFile();

            IDirectoryObject targetDirectory = new MemoryDirectoryObject("targetDir");

            var mock = new Mock<ITaskQueue>();

            var syncNet = new Processor(sourceDirectory, targetDirectory, mock.Object);
            syncNet.ProcessSourceDirectory();

            var newName = "newName.txt";
            var sourceFile = sourceDirectory.GetFiles().First();

            syncNet.RenameFile(sourceFile, newName);

            var targetFile = targetDirectory.GetFiles().First();
            mock.Verify(x => x.Queue(
                It.Is<RenameFileTask>(t => t.File.FullName == targetFile.FullName && t.NewName == newName))
                );
        }

        [TestMethod]
        public void RenamesDirectory()
        {
            IDirectoryObject sourceDirectory = DirectoryHelper.CreateFullDirectory();

            IDirectoryObject targetDirectory = new MemoryDirectoryObject("targetDir");

            var mock = new Mock<ITaskQueue>();

            var syncNet = new Processor(sourceDirectory, targetDirectory, mock.Object);
            syncNet.ProcessSourceDirectory();

            var newName = "newName";
            var subDirectory = sourceDirectory.GetDirectories().First();

            syncNet.RenameDirectory(subDirectory, newName);

            var targetFile = targetDirectory.GetDirectories().First();
            mock.Verify(x => x.Queue(
                It.Is<RenameDirectoryTask>(t => t.Directory.FullName == targetFile.FullName && t.NewName == newName))
            );
        }

        [TestMethod]
        public void CountsFilesLeft()
        {
            var sourceDirectory = DirectoryHelper.CreateFullDirectory();

            var targetDirectory = new MemoryDirectoryObject("directory");

            var queue = new ManualTaskQueue();
            var syncNet = new Processor(sourceDirectory, targetDirectory, queue);

            var progressUpdates = new List<SyncNetProgressChangedEventArgs>();
            syncNet.ProgressChanged += delegate(Processor sender, SyncNetProgressChangedEventArgs e)
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

        [TestMethod]
        public void RemovesFileFromQueueWhenDeleted()
        {
            var sourceDirectory = DirectoryHelper.CreateDirectoryWithFiles();

            var targetDirectory = new MemoryDirectoryObject("targetDirectory");

            var queue = new ManualTaskQueue();
            var syncNet = new Processor(sourceDirectory, targetDirectory, queue);
            syncNet.ProcessSourceDirectory();

            var fileToDelete = sourceDirectory.GetFiles().First();
            (fileToDelete as MemoryFileObject).Exists = false;

            queue.ExecuteAll();

            var fileObjects = targetDirectory.GetFiles().Where(x => x.Exists);

            Assert.AreEqual(1, fileObjects.Count());
            Assert.AreEqual(DirectoryHelper.FileName2, fileObjects.First().Name);
        }
    }
}