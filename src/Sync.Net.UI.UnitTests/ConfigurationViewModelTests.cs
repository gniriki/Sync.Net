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
        private Mock<IConfigurationTester> _configurationTester;
        private ConfigurationViewModel _configurationViewModel;
        private readonly string _testDirectory = "C:\\Test\\Dir";

        private Mock<IWindowManager> _windowManager;

        [TestInitialize]
        public void Initialize()
        {
            _configuration = new Mock<SyncNetConfiguration>();
            _configurationTester = new Mock<IConfigurationTester>();
            _windowManager = new Mock<IWindowManager>();
            _configFile = new Mock<IConfigFile>();
            _windowManager.Setup(x => x.ShowDirectoryDialog()).Returns(_testDirectory);

            _configurationTester.Setup(x => x.Test(It.IsAny<SyncNetConfiguration>())).Returns(
                new ConfigurationTestResult
                {
                    IsValid = true
                });

            _configurationViewModel =
                new ConfigurationViewModel(_configuration.Object,
                _windowManager.Object,
                _configFile.Object,
                _configurationTester.Object);
        }

        [TestMethod]
        public void SetsSelectedDirectory()
        {
            _configurationViewModel.SelectFile.Execute(null);
            Assert.AreEqual(_testDirectory, _configurationViewModel.LocalDirectory);
        }

        [TestMethod]
        public void SaveSavesConfiguration()
        {
            _configurationViewModel.Save.Execute(null);
            _configuration.Verify(x => x.Save(It.IsAny<Stream>()));
        }

        [TestMethod]
        public void SaveRestartsTheApplication()
        {
            _configurationViewModel.Save.Execute(null);
            _windowManager.Verify(x => x.RestartApplication());
        }

        [TestMethod]
        public void SaveTestsConfiguration()
        {
            _configurationViewModel.Save.Execute(null);
            _configurationTester.Verify(x => x.Test(It.IsAny<SyncNetConfiguration>()));
        }

        [TestMethod]
        public void SaveIsCancelledIfConfigurationIsInvalid()
        {
            string invalidConfigurationMessage = "invalid";

            _configurationTester.Setup(x => x.Test(It.IsAny<SyncNetConfiguration>())).Returns(
                new ConfigurationTestResult
                {
                    IsValid = false,
                    Message = invalidConfigurationMessage
                });

            _configurationViewModel.Save.Execute(null);
            _configuration.Verify(x => x.Save(It.IsAny<Stream>()), Times.Never);
            _windowManager.Verify(x => x.RestartApplication(), Times.Never);
        }

        [TestMethod]
        public void TestConfigurationShowsErrorIfConfigurationIsInvalid()
        {
            string invalidConfigurationMessage = "invalid";

            _configurationTester.Setup(x => x.Test(It.IsAny<SyncNetConfiguration>())).Returns(
                new ConfigurationTestResult
                {
                    IsValid = false,
                    Message = invalidConfigurationMessage
                });

            _configurationViewModel.Test.Execute(null);
            _windowManager.Verify(x => x.ShowMessage(invalidConfigurationMessage));
        }

        [TestMethod]
        public void TestConfigurationShowsOkMessageIfConfigurationIsInvalid()
        {
            _configurationViewModel.Test.Execute(null);
            _windowManager.Verify(x => x.ShowMessage("Ok"));
        }
    }
}