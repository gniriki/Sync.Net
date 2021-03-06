﻿using System;
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
        private Mock<IConfigurationTester> _configurationTester;
        private ConfigurationViewModel _configurationViewModel;
        private readonly string _testDirectory = "C:\\Test\\Dir";

        private Mock<IWindowManager> _windowManager;
        private Mock<IConfigurationProvider> _configurationProvider;
        private Mock<IProcessorConfigurationValidator> _configurationValidator;

        [TestInitialize]
        public void Initialize()
        {
            _configurationTester = new Mock<IConfigurationTester>();
            _windowManager = new Mock<IWindowManager>();
            _configFile = new Mock<IConfigFile>();
            _configurationProvider = new Mock<IConfigurationProvider>();
            _configurationValidator = new Mock<IProcessorConfigurationValidator>();

            _windowManager.Setup(x => x.ShowDirectoryDialog()).Returns(_testDirectory);
            _configurationProvider.Setup(x => x.Current).Returns(new ProcessorConfiguration());
            _configurationTester.Setup(x => x.Test(It.IsAny<ProcessorConfiguration>())).Returns(
                new ConfigurationTestResult
                {
                    TestPassed = true
                });

            _configurationValidator.Setup(x => x.Validate(It.IsAny<ProcessorConfiguration>())).Returns(
                new ProcessorConfigurationValidationResult
                {
                    IsValid = true
                });

            _configurationViewModel =
                new ConfigurationViewModel(_configurationProvider.Object,
                _windowManager.Object,
                _configFile.Object,
                _configurationTester.Object,
                _configurationValidator.Object);
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
            _configurationProvider.Verify(x => x.Save());
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
            _configurationTester.Verify(x => x.Test(It.IsAny<ProcessorConfiguration>()));
        }

        [TestMethod]
        public void SaveIsCancelledIfConfigurationTestFails()
        {
            string testFailedMessage = "failed";

            _configurationTester.Setup(x => x.Test(It.IsAny<ProcessorConfiguration>())).Returns(
                new ConfigurationTestResult
                {
                    TestPassed = false,
                    Message = testFailedMessage
                });

            _configurationViewModel.Save.Execute(null);
            _configurationProvider.Verify(x => x.Save(), Times.Never);
            _windowManager.Verify(x => x.RestartApplication(), Times.Never);
        }

        [TestMethod]
        public void TestConfigurationShowsErrorIfConfigurationTestFails()
        {
            string testFailedMessage = "failed";

            _configurationTester.Setup(x => x.Test(It.IsAny<ProcessorConfiguration>())).Returns(
                new ConfigurationTestResult
                {
                    TestPassed = false,
                    Message = testFailedMessage
                });

            _configurationViewModel.Test.Execute(null);
            _windowManager.Verify(x => x.ShowMessage(testFailedMessage));
        }

        [TestMethod]
        public void SaveIsCancelledIfConfigurationIsInvalid()
        {
            string invalidConfigurationMessage = "invalid";

            _configurationValidator.Setup(x => x.Validate(It.IsAny<ProcessorConfiguration>())).Returns(
                new ProcessorConfigurationValidationResult
                {
                    IsValid = false,
                    Message = invalidConfigurationMessage
                });

            _configurationViewModel.Save.Execute(null);
            _configurationProvider.Verify(x => x.Save(), Times.Never);
            _windowManager.Verify(x => x.RestartApplication(), Times.Never);
        }

        [TestMethod]
        public void TestConfigurationShowsErrorIfConfigurationIsInvalid()
        {
            string invalidConfigurationMessage = "invalid";

            _configurationValidator.Setup(x => x.Validate(It.IsAny<ProcessorConfiguration>())).Returns(
                new ProcessorConfigurationValidationResult
                {
                    IsValid = false,
                    Message = invalidConfigurationMessage
                });

            _configurationViewModel.Test.Execute(null);
            _windowManager.Verify(x => x.ShowMessage(invalidConfigurationMessage));
        }

        [TestMethod]
        public void TestConfigurationShowsOkMessage()
        {
            _configurationViewModel.Test.Execute(null);
            _windowManager.Verify(x => x.ShowMessage("Ok"));
        }
    }
}