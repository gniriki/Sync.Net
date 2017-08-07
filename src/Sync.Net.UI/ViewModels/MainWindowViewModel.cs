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
        private readonly SyncNetWatcher _syncNetWatcher;
        public ICommand ExitCommand { get; }
        public AsyncCommand SyncCommand { get; }

        public string Log => _log;

        public MainWindowViewModel(IWindowManager windowManager, 
            ISyncNetTaskFactory taskFactory,
            SyncNetConfiguration configuration,
            IFileWatcher watcher)
        {
            _log = string.Empty;

            _syncNetWatcher = new SyncNetWatcher(taskFactory, configuration, watcher);
            _syncNetWatcher.Log = WriteToLog;
            _syncNetWatcher.Watch();

            ExitCommand = new RelayCommand(
                p => true,
                p =>
                {
                    windowManager.ShutdownApplication();
                });

            SyncCommand = new AsyncCommand(_syncNetWatcher.Sync,
                () => true);
        }

        private void WriteToLog(string line)
        {
            _log += $"{DateTime.Now}: {line}\n";
            OnPropertyChanged(nameof(Log));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
