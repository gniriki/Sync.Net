using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sync.Net.Configuration;
using Sync.Net.IO;

namespace Sync.Net.Tests
{
    [TestClass]
    public class SyncNetWatcherTests
    {
        private SyncNetConfiguration _configuration;
        private Mock<IFileWatcher> _fileWatcher;
        private Mock<ISyncNetTask> _task;

        [TestInitialize]
        public void Initialize()
        {
            _task = new Mock<ISyncNetTask>();
            _configuration = new SyncNetConfiguration();
            _fileWatcher = new Mock<IFileWatcher>();
        }

        [TestMethod]
        public void WatchesForFileChanges()
        {
            _configuration.LocalDirectory = "dir";

            var watcher = new SyncNetWatcher(_task.Object, _configuration, _fileWatcher.Object);
            watcher.Watch();

            _fileWatcher.Verify(x => x.WatchForChanges(_configuration.LocalDirectory));
        }

        [TestMethod]
        public void UploadsFileWhenCreated()
        {
            _configuration.LocalDirectory = "dir";
            var filename = "file";

            var wasUploaded = false;
            _task.Setup(x => x.ProcessFileAsync(It.IsAny<IFileObject>())).Callback(() => wasUploaded = true);

            var watcher = new SyncNetWatcher(_task.Object, _configuration, _fileWatcher.Object);
            watcher.Watch();

            _fileWatcher.Raise(x => x.Created += null,
                new FileSystemEventArgs(WatcherChangeTypes.Created, _configuration.LocalDirectory, filename));

            Assert.IsTrue(wasUploaded);
        }

        [TestMethod]
        public void UploadsDirectoryWhenCreated()
        {
            _configuration.LocalDirectory = "dir";
            var filename = "file";

            var wasUploaded = false;
            _task.Setup(x => x.ProcessDirectoryAsync(It.IsAny<IDirectoryObject>())).Callback(() => wasUploaded = true);

            _fileWatcher.Setup(x => x.IsDirectory(It.IsAny<string>())).Returns(true);

            var watcher = new SyncNetWatcher(_task.Object, _configuration, _fileWatcher.Object);
            watcher.Watch();

            _fileWatcher.Raise(x => x.Created += null,
                new FileSystemEventArgs(WatcherChangeTypes.Created, _configuration.LocalDirectory, filename));

            Assert.IsTrue(wasUploaded);
        }
    }
}