using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sync.Net.Configuration;
using Sync.Net.UI.Utils;
using Sync.Net.UI.ViewModels;

namespace Sync.Net.UI.UnitTests
{
    [TestClass]
    public class MainWindowViewModelTests
    {
        private Mock<IWindowManager> _windowManager;

        [TestMethod]
        public void ExitCommandShutDownApplication()
        {
            bool shutdown = false;

            _windowManager = new Moq.Mock<IWindowManager>();
            _windowManager.Setup(x => x.ShutdownApplication()).Callback(() => shutdown = true);

            MainWindowViewModel model = new MainWindowViewModel(_windowManager.Object, null, null);
            model.ExitCommand.Execute(null);

            Assert.IsTrue(shutdown);
        }

        [TestMethod]
        public void SyncCommandStartsSync()
        {
            var task = new Moq.Mock<ISyncNetTask>();
            bool wasRun = false;
            task.Setup(x => x.Run()).Callback(() => wasRun = true);
            var taskFactory = new Moq.Mock<ISyncNetTaskFactory>();
            taskFactory.Setup(x => x.Create(It.IsAny<SyncNetConfiguration>())).Returns(task.Object);
            MainWindowViewModel model = new MainWindowViewModel(null, taskFactory.Object, new SyncNetConfiguration());
            model.SyncCommand.Execute(null);

            Assert.IsTrue(wasRun);

        }
    }
}
