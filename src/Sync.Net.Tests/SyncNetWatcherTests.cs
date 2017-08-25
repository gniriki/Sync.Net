using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sync.Net.Configuration;

namespace Sync.Net.Tests
{
    [TestClass]
    public class SyncNetWatcherTests
    {
        private Mock<ISyncNetTask> _task;
        private SyncNetConfiguration _configuration;
        private Mock<IFileWatcher> _fileWatcher;

        [TestInitialize]
        public void Initialize()
        {
            _task = new Moq.Mock<ISyncNetTask>();
            _configuration = new SyncNetConfiguration();
            _fileWatcher = new Mock<IFileWatcher>();
        }

        [TestMethod]
        public void WatchesForFileChanges()
        {
            _configuration.LocalDirectory = "dir";
            bool isWatching = false;
            _fileWatcher.Setup(x => x.WatchForChanges(_configuration.LocalDirectory)).Callback(() => isWatching = true);

            var watcher = new SyncNetWatcher(_task.Object, _configuration, _fileWatcher.Object);
            watcher.Watch();

            Assert.IsTrue(isWatching);
        }

        [TestMethod]
        public void UploadsFileWhenCreated()
        {
            _configuration.LocalDirectory = "dir";
            var filename = "file";

            bool wasUploaded = false;
            _task.Setup(x => x.ProcessFileAsync(It.IsAny<string>())).Callback(() => wasUploaded = true);

            var watcher = new SyncNetWatcher(_task.Object, _configuration, _fileWatcher.Object);
            watcher.Watch();

            _fileWatcher.Raise(x => x.Created += null, new FileSystemEventArgs(WatcherChangeTypes.Created, _configuration.LocalDirectory, filename));

            Assert.IsTrue(wasUploaded);
        }

        [TestMethod]
        public void UploadsDirectoryWhenCreated()
        {
            _configuration.LocalDirectory = "dir";
            var filename = "file";

            bool wasUploaded = false;
            _task.Setup(x => x.ProcessDirectoryAsync(It.IsAny<string>())).Callback(() => wasUploaded = true);

            _fileWatcher.Setup(x => x.IsDirectory(It.IsAny<string>())).Returns(true);

            var watcher = new SyncNetWatcher(_task.Object, _configuration, _fileWatcher.Object);
            watcher.Watch();

            _fileWatcher.Raise(x => x.Created += null, new FileSystemEventArgs(WatcherChangeTypes.Created, _configuration.LocalDirectory, filename));

            Assert.IsTrue(wasUploaded);
        }
    }
}
