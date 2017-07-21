using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sync.Net.Configuration;
using Sync.Net.UI.Utils;

namespace Sync.Net.UI.ViewModels
{
    public class MainWindowViewModel
    {
        private IWindowManager _windowManager;
        private ISyncNetTaskFactory _taskFactory;
        public ICommand ExitCommand { get; }
        public ICommand SyncCommand { get; }

        public MainWindowViewModel(IWindowManager windowManager, 
            ISyncNetTaskFactory taskFactory,
            SyncNetConfiguration configuration)
        {
            _windowManager = windowManager;
            _taskFactory = taskFactory;
            ExitCommand = new RelayCommand(
                p => true,
                p =>
                {
                    _windowManager.ShutdownApplication();
                });

            SyncCommand = new RelayCommand(
                p => true,
                p =>
                {
                    var task = _taskFactory.Create(configuration);
                    task.Run();
                });
        }
    }
}
