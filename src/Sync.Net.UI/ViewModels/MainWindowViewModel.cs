using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sync.Net.UI.Utils;

namespace Sync.Net.UI.ViewModels
{
    public class MainWindowViewModel
    {
        private IWindowManager _windowManager;
        public ICommand ExitCommand { get; }

        public MainWindowViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
            ExitCommand = new RelayCommand(
                p => true,
                p =>
                {
                    _windowManager.ShutdownApplication();
                });
        }
    }
}
