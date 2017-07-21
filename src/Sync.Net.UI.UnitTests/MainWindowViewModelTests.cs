using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sync.Net.UI.Utils;
using Sync.Net.UI.ViewModels;

namespace Sync.Net.UI.UnitTests
{
    [TestClass]
    public class MainWindowViewModelTests
    {
        private Mock<IWindowManager> _windowManager;

        [TestMethod]
        public void SyncCommandStartsSync()
        {
            bool shutdown = false;

            _windowManager = new Moq.Mock<IWindowManager>();
            _windowManager.Setup(x => x.ShutdownApplication()).Callback(() => shutdown = true);

            MainWindowViewModel model = new MainWindowViewModel(_windowManager.Object);
            model.ExitCommand.Execute(null);

            Assert.IsTrue(shutdown);
        }
    }
}
