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
            watcher.Start();

            _fileWatcher.Verify(x => x.WatchForChanges(_configuration.LocalDirectory));
        }

        [TestMethod]
        public void UploadsFileWhenCreated()
        {
            _configuration.LocalDirectory = "dir";
            var filename = "file";

            var wasUploaded = false;
            _processor.Setup(x => x.CopyFile(It.IsAny<IFileObject>())).Callback(() => wasUploaded = true);

            var watcher = new EventWatcher(_processor.Object, _configurationProvider.Object, _fileWatcher.Object);
            watcher.Start();

            _fileWatcher.Raise(x => x.Created += null,
                new FileSystemEventArgs(WatcherChangeTypes.Created, _configuration.LocalDirectory, filename));

            Assert.IsTrue(wasUploaded);
        }

        [TestMethod]
        public void RenamesFile()
        {
            _configuration.LocalDirectory = "c:\\dir";

            var watcher = new EventWatcher(_processor.Object, _configurationProvider.Object, _fileWatcher.Object);
            watcher.Start();

            //_processor.Setup(x => x.RenameFile(It.IsAny<IFileObject>(), It.IsAny<string>()))
            //    .Callback<IFileObject, string>((file, name) =>
            //    {
            //        var test = file;
            //        var test2 = name;
            //    });

            _fileWatcher.Raise(x => x.Renamed += null,
                new RenamedEventArgs(WatcherChangeTypes.Renamed, _configuration.LocalDirectory, "newName.txt", "oldName.txt"));

            var oldFullPath = Path.Combine(_configuration.LocalDirectory, "oldName.txt");

            _processor.Verify(x => x.RenameFile(
                It.Is<IFileObject>(f => f.FullName == oldFullPath), 
                "newName.txt"));
        }

        [TestMethod]
        public void UploadsDirectoryWhenCreated()
        {
            _configuration.LocalDirectory = "dir";
            var filename = "file";

            _fileWatcher.Setup(x => x.IsDirectory(It.IsAny<string>())).Returns(true);

            var watcher = new EventWatcher(_processor.Object, _configurationProvider.Object, _fileWatcher.Object);
            watcher.Start();

            _fileWatcher.Raise(x => x.Created += null,
                new FileSystemEventArgs(WatcherChangeTypes.Created, _configuration.LocalDirectory, filename));

            _processor.Verify(x => x.ProcessDirectory(It.IsAny<IDirectoryObject>()));
        }
    }
}