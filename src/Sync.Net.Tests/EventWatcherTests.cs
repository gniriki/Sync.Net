using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sync.Net.Configuration;
using Sync.Net.IO;

namespace Sync.Net.Tests
{
    [TestClass]
    public class EventWatcherTests
    {
        private ProcessorConfiguration _configuration;
        private Mock<IFileWatcher> _fileWatcher;
        private Mock<IProcessor> _processor;
        private Mock<IConfigurationProvider> _configurationProvider;

        [TestInitialize]
        public void Initialize()
        {
            _processor = new Mock<IProcessor>();
            _configuration = new ProcessorConfiguration();
            _fileWatcher = new Mock<IFileWatcher>();
            _configurationProvider = new Mock<IConfigurationProvider>();

            _configurationProvider.Setup(x => x.Current).Returns(_configuration);
        }

        [TestMethod]
        public void WatchesForFileChanges()
        {
            _configuration.LocalDirectory = "dir";

            var watcher = new EventWatcher(_processor.Object, _configurationProvider.Object, _fileWatcher.Object);
            watcher.Watch();

            _fileWatcher.Verify(x => x.WatchForChanges(_configuration.LocalDirectory));
        }

        [TestMethod]
        public void UploadsFileWhenCreated()
        {
            _configuration.LocalDirectory = "dir";
            var filename = "file";

            var wasUploaded = false;
            _processor.Setup(x => x.CopyFileAsync(It.IsAny<IFileObject>())).Callback(() => wasUploaded = true);

            var watcher = new EventWatcher(_processor.Object, _configurationProvider.Object, _fileWatcher.Object);
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
            _processor.Setup(x => x.ProcessDirectoryAsync(It.IsAny<IDirectoryObject>())).Callback(() => wasUploaded = true);

            _fileWatcher.Setup(x => x.IsDirectory(It.IsAny<string>())).Returns(true);

            var watcher = new EventWatcher(_processor.Object, _configurationProvider.Object, _fileWatcher.Object);
            watcher.Watch();

            _fileWatcher.Raise(x => x.Created += null,
                new FileSystemEventArgs(WatcherChangeTypes.Created, _configuration.LocalDirectory, filename));

            Assert.IsTrue(wasUploaded);
        }
    }
}