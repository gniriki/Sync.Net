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
        private SyncNetConfiguration _configuration;
        private Mock<ILogger> _logger;
        private Mock<ISyncNetTask> _task;
        private Mock<IWindowManager> _windowManager;

        [TestInitialize]
        public void Initialize()
        {
            _windowManager = new Mock<IWindowManager>();
            _task = new Mock<ISyncNetTask>();
            _configuration = new SyncNetConfiguration();
            _logger = new Mock<ILogger>();
        }

        [TestMethod]
        public void ExitCommandShutDownApplication()
        {
            var shutdown = false;

            _windowManager.Setup(x => x.ShutdownApplication()).Callback(() => shutdown = true);

            var model =
                new MainWindowViewModel(_windowManager.Object, _task.Object, _configuration, _logger.Object);
            model.ExitCommand.Execute(null);

            Assert.IsTrue(shutdown);
        }

        [TestMethod]
        public async Task SyncCommandStartsSync()
        {
            var wasRun = false;
            _task.Setup(x => x.ProcessFilesAsync())
                .Callback(() => { wasRun = true; })
                .Returns(() => Task.CompletedTask);

            var model =
                new MainWindowViewModel(null, _task.Object, _configuration, _logger.Object);

            await model.SyncCommand.Execute();
            Assert.IsTrue(wasRun);
        }
    }
}