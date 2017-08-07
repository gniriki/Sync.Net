using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sync.Net.Configuration;
using Sync.Net.UI.Annotations;
using Sync.Net.UI.Utils;

namespace Sync.Net.UI.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _log;
        private ISyncNetTask _task;
        private ILogger _logger;
        public ICommand ExitCommand { get; }
        public AsyncCommand SyncCommand { get; }

        public string Log => _log;

        public MainWindowViewModel(IWindowManager windowManager, 
            ISyncNetTaskFactory taskFactory,
            SyncNetConfiguration configuration,
            ILogger logger)
        {
            _logger = logger;
            _logger.LogUpdated += _logger_LogUpdated;
            _log = logger.Contents;
            _task = taskFactory.Create(configuration);

            ExitCommand = new RelayCommand(
                p => true,
                p =>
                {
                    windowManager.ShutdownApplication();
                });

            SyncCommand = new AsyncCommand(Sync,
                () => true);
        }

        private void _logger_LogUpdated(string newLine)
        {
            _log += newLine;
            OnPropertyChanged(nameof(Log));
        }

        public async Task Sync()
        {
            await Task.Run(() => _task.Run());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
