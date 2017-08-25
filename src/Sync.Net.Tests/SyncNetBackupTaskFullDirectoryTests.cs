using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sync.Net.TestHelpers;

namespace Sync.Net.Tests
{
    public class SyncNetBackupTaskFullDirectoryTests
    {
        protected string _contents;
        protected string _fileName;
        protected string _fileName2;
        protected MemoryDirectoryObject _sourceDirectory;
        protected string _subDirectoryName;
        protected string _subFileName;
        protected string _subFileName2;
        protected SyncNetBackupTask _syncNet;
        protected MemoryDirectoryObject _targetDirectory;

        [TestInitialize]
        public void Initialize()
        {
            _fileName = "file.txt";
            _fileName2 = "file2.txt";
            _subFileName = "subFile.txt";
            _subFileName2 = "subfile2.txt";
            _subDirectoryName = "dir";
            _contents = "This is file content";

            _sourceDirectory = new MemoryDirectoryObject("sourceDirectory")
                .AddFile(_fileName, _contents)
                .AddFile(_fileName2, _contents);

            _sourceDirectory.AddDirectory(new MemoryDirectoryObject(_subDirectoryName, _sourceDirectory.FullName)
                .AddFile(_subFileName, _contents)
                .AddFile(_subFileName2, _contents));

            _targetDirectory = new MemoryDirectoryObject("targetDirectory");

            _syncNet = new SyncNetBackupTask(_sourceDirectory, _targetDirectory);
        }
    }
}