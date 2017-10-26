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
        private ProcessorConfiguration _configuration;
        private Mock<ILogger> _logger;
        private Mock<IProcessor> _processor;
        private Mock<IWindowManager> _windowManager;

        [TestInitialize]
        public void Initialize()
        {
            _windowManager = new Mock<IWindowManager>();
            _configuration = new ProcessorConfiguration();
            _logger = new Mock<ILogger>();
        }

        [TestMethod]
        public void ExitCommandShutDownApplication()
        {
            var model =
                new MainWindowViewModel(_windowManager.Object, _logger.Object);
            model.ExitCommand.Execute(null);

            _windowManager.Verify(x => x.ShutdownApplication());
        }

        [TestMethod]
        public void ConfigureCommandOpensConfigurationWindow()
        {
            var model =
                new MainWindowViewModel(_windowManager.Object, _logger.Object);

            model.ConfigureCommand.Execute(null);
            _windowManager.Verify(x => x.ShowConfiguration());
        }
    }
}