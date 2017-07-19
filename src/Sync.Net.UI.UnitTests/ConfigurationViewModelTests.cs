using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sync.Net.Configuration;
using Sync.Net.UI.Utils;
using Sync.Net.UI.ViewModels;

namespace Sync.Net.UI.UnitTests
{
    [TestClass]
    public class ConfigurationViewModelTests
    {
        private string _testDirectory = "C:\\Test\\Dir";

        [TestMethod]
        public void SavesSelectedPath()
        {
            var configuration = new SyncNetConfiguration();
            var windowManager = new Moq.Mock<IWindowManager>();
            windowManager.Setup(x => x.ShowDirectoryDialog()).Returns(_testDirectory);
            var model = new ConfigurationViewModel(configuration, windowManager.Object);

            model.SelectFile.Execute(null);

            Assert.AreEqual(_testDirectory, configuration.LocalDirectory);
        }
    }
}
