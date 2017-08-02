using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sync.Net.IO;
using Sync.Net.TestHelpers;

namespace Sync.Net.Tests
{
    [TestClass]
    public class SyncNetBackupTaskUploadFileTests
    {
        private string _contents;
        private string _fileName;
        private string _fileName2;
        private string _subDirectoryName;
        private string _subFileName;
        private string _subFileName2;
        private IDirectoryObject _sourceDirectory;
        private MemoryDirectoryObject _targetDirectory;
        private SyncNetBackupTask _syncNet;

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

            _targetDirectory = new MemoryDirectoryObject("directory");

            _syncNet = new SyncNetBackupTask(_sourceDirectory, _targetDirectory);
        }

        [TestMethod]
        public void UploadsFileByName()
        {
            _syncNet.UpdateFile(_fileName);

            var files = _targetDirectory.GetFiles();
            Assert.AreEqual(_fileName, files.First().Name);
        }

        [TestMethod]
        public void UploadsFileFromSubfolderDoesntCreateAFileInMainDirectory()
        {
            _syncNet.UpdateFile(_subDirectoryName + "\\" + _subFileName);

            var files = _targetDirectory.GetFiles();

            Assert.IsTrue(!files.Any()); 
        }

        [TestMethod]
        public void UploadsFileFromSubfolderCreatesASubDirectory()
        {
            _syncNet.UpdateFile(_subDirectoryName + "\\" + _subFileName);

            var dirs = _targetDirectory.GetDirectories();

            Assert.IsTrue(dirs.Count() == 1);
            Assert.AreEqual(_subFileName, dirs.First().GetFiles().First().Name);
        }
    }
}
