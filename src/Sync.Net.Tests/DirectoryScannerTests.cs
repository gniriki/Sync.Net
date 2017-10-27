using System;
using System.Collections.Generic;
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
    public class DirectoryScannerTests
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
        public void ReturnsFilesNotInTargetDirectory()
        {
            var sourceDirectory = new MemoryDirectoryObject("directory")
                .AddFile(_fileName, _contents)
                .AddFile(_fileName2, _contents);

            sourceDirectory.AddDirectory(new MemoryDirectoryObject(_subDirectoryName, sourceDirectory.FullName)
                .AddFile(_subFileName, _contents)
                .AddFile(_subFileName2, _contents));

            IDirectoryObject targetDirectory = new MemoryDirectoryObject("directory");

            var scanner = new DirectoryScanner(sourceDirectory, targetDirectory);
            var filesToCopy = scanner.GetFilesToCopy();

            Assert.AreEqual(4, filesToCopy.Count);
            Assert.IsTrue(filesToCopy.Any(x => x.Name == _fileName));
            Assert.IsTrue(filesToCopy.Any(x => x.Name == _fileName2));
            Assert.IsTrue(filesToCopy.Any(x => x.Name == _subFileName));
            Assert.IsTrue(filesToCopy.Any(x => x.Name == _subFileName2));
        }

        [TestMethod]
        public void ReturnsOnlyNewerFiles()
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

            var scanner = new DirectoryScanner(sourceDirectory, targetDirectory);
            var filesToCopy = scanner.GetFilesToCopy();


            Assert.AreEqual(1, filesToCopy.Count);
            Assert.IsTrue(filesToCopy.Any(x => x.Name == _fileName2));
        }
    }
}
