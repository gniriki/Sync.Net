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
        private Mock<ISyncNetTaskFactory> _taskFactory;

        [TestInitialize]
        public void Initialize()
        {
            _task = new Moq.Mock<ISyncNetTask>();
            _configuration = new SyncNetConfiguration();
            _fileWatcher = new Mock<IFileWatcher>();
            _taskFactory = new Moq.Mock<ISyncNetTaskFactory>();
            _taskFactory.Setup(x => x.Create(It.IsAny<SyncNetConfiguration>())).Returns(_task.Object);
        }

        [TestMethod]
        public void WatchesForFileChanges()
        {
            _configuration.LocalDirectory = "dir";
            bool isWatching = false;
            _fileWatcher.Setup(x => x.WatchForChanges(_configuration.LocalDirectory)).Callback(() => isWatching = true);

            var watcher = new SyncNetWatcher(_taskFactory.Object, _configuration, _fileWatcher.Object);
            watcher.Watch();

            Assert.IsTrue(isWatching);
        }

        [TestMethod]
        public void UploadsFileWhenCreated()
        {
            _configuration.LocalDirectory = "dir";
            var filename = "file";

            bool wasUploaded = false;
            _task.Setup(x => x.UpdateFile(It.IsAny<string>())).Callback(() => wasUploaded = true);

            var taskFactory = new Moq.Mock<ISyncNetTaskFactory>();
            taskFactory.Setup(x => x.Create(It.IsAny<SyncNetConfiguration>())).Returns(_task.Object);

            var watcher = new SyncNetWatcher(_taskFactory.Object, _configuration, _fileWatcher.Object);
            watcher.Watch();

            _fileWatcher.Raise(x => x.Created += null, new FileSystemEventArgs(WatcherChangeTypes.Created, _configuration.LocalDirectory, filename));

            Assert.IsTrue(wasUploaded);
        }

        [TestMethod]
        public void MakesRelativePathWhenUploadingTheFile()
        {
            _configuration.LocalDirectory = "c:\\dir";
            var raiseDir = _configuration.LocalDirectory + "\\sub";
            var raiseFilename = "file";

            var uploadedFilePath = string.Empty;
            _task.Setup(x => x.UpdateFile(It.IsAny<string>())).Callback<string>(path => uploadedFilePath = path);

            var taskFactory = new Moq.Mock<ISyncNetTaskFactory>();
            taskFactory.Setup(x => x.Create(It.IsAny<SyncNetConfiguration>())).Returns(_task.Object);

            var watcher = new SyncNetWatcher(_taskFactory.Object, _configuration, _fileWatcher.Object);
            watcher.Watch();

            _fileWatcher.Raise(x => x.Created += null, new FileSystemEventArgs(WatcherChangeTypes.Created, raiseDir, raiseFilename));

            Assert.AreEqual("\\sub\\file", uploadedFilePath);
        }
    }
}
