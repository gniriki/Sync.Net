using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sync.Net.Configuration;
using Sync.Net.UI.Utils;
using Sync.Net.UI.ViewModels;

namespace Sync.Net.UI.UnitTests
{
    [TestClass]
    public class ConfigurationViewModelTests
    {
        private Mock<IConfigFile> _configFile;
        private Mock<SyncNetConfiguration> _configuration;
        private ConfigurationViewModel _configurationViewModel;
        private readonly string _testDirectory = "C:\\Test\\Dir";
        private bool _wasSaved;
        private Mock<IWindowManager> _windowManager;

        [TestMethod]
        public void SetsSelectedDirectory()
        {
            _configurationViewModel.SelectFile.Execute(null);
            Assert.AreEqual(_testDirectory, _configurationViewModel.LocalDirectory);
        }

        [TestInitialize]
        public void Initialize()
        {
            _wasSaved = false;

            _configuration = new Mock<SyncNetConfiguration>();
            _windowManager = new Mock<IWindowManager>();
            _configFile = new Mock<IConfigFile>();
            _windowManager.Setup(x => x.ShowDirectoryDialog()).Returns(_testDirectory);
            _configuration.Setup(x => x.Save(It.IsAny<Stream>())).Callback(() => _wasSaved = true);

            _configurationViewModel =
                new ConfigurationViewModel(_configuration.Object, _windowManager.Object, _configFile.Object);
        }


        [TestMethod]
        public void SavesOnPropertyChanged()
        {
            _configurationViewModel.LocalDirectory = "test";
            Assert.IsTrue(_wasSaved);
        }
    }
}