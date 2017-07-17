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
        [TestMethod]
        public void CreatesFileInTargetDirectory()
        {
            SyncNet syncNet = new SyncNet();
            var memoryDirectoryObject = new MemoryDirectoryObject();

            syncNet.Backup(new MemoryFileObject("file.txt"), memoryDirectoryObject);
            Assert.IsTrue(memoryDirectoryObject.ContainsFile("file.txt"));
        }

        [TestMethod]
        public void WritesFileContentToTargetFile()
        {
            SyncNet syncNet = new SyncNet();
            IDirectoryObject memoryDirectoryObject = new MemoryDirectoryObject();

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
            var fileName = "file.txt";
            var fileName2 = "file2.txt";
            var subDirectory = "dir";
            var contents = "This is file content";

            IDirectoryObject sourceDirectory = new MemoryDirectoryObject()
                .AddDirectory(new MemoryDirectoryObject()
                    .AddFile(fileName, contents)
                    .AddFile(fileName2, contents));

            IDirectoryObject targetDirectory = new MemoryDirectoryObject();

            SyncNet syncNet = new SyncNet();
            syncNet.Backup(sourceDirectory, targetDirectory);

            Assert.AreEqual(0, targetDirectory.GetFiles().Count());

            IEnumerable<IDirectoryObject> subDirectories = targetDirectory.GetDirectories();
            Assert.AreEqual(1, subDirectories.Count());

            IEnumerable<IFileObject> files = subDirectories.First().GetFiles();
            Assert.AreEqual(2, files.Count());

            Assert.IsTrue(files.Any(x => x.Name == fileName));
            Assert.IsTrue(files.Any(x => x.Name == fileName2));
        }

        [TestMethod]
        public void UploadsFilesToDirectory()
        {
            var fileName = "file.txt";
            var fileName2 = "file2.txt";
            var contents = "This is file content";

            IDirectoryObject sourceDirectory = new MemoryDirectoryObject()
                .AddFile(fileName, contents)
                .AddFile(fileName2, contents);

            IDirectoryObject targetDirectory = new MemoryDirectoryObject();

            SyncNet syncNet = new SyncNet();
            syncNet.Backup(sourceDirectory, targetDirectory);

            IEnumerable<IFileObject> files = targetDirectory.GetFiles();
            Assert.AreEqual(2, files.Count());

            Assert.IsTrue(files.Any(x => x.Name == fileName));
            Assert.IsTrue(files.Any(x => x.Name == fileName2));
        }
    }
}
