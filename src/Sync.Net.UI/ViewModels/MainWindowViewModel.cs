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
        private IWindowManager _windowManager;
        private ISyncNetTaskFactory _taskFactory;
        private string _log;
        private IFileWatcher _watcher;
        private ISyncNetTask _task;
        public ICommand ExitCommand { get; }
        public AsyncCommand SyncCommand { get; }

        public string Log => _log;

        public MainWindowViewModel(IWindowManager windowManager, 
            ISyncNetTaskFactory taskFactory,
            SyncNetConfiguration configuration,
            IFileWatcher watcher)
        {
            _windowManager = windowManager;
            _taskFactory = taskFactory;
            _log = string.Empty;
            _watcher = watcher;
            _task = _taskFactory.Create(configuration);
            _task.ProgressChanged += Task_ProgressChanged;

            _watcher.Created += (sender, args) =>
            {
                _task.UpdateFile(args.FullPath);
            };

            _watcher.WatchForChanges(configuration.LocalDirectory);

            ExitCommand = new RelayCommand(
                p => true,
                p =>
                {
                    _windowManager.ShutdownApplication();
                });

            SyncCommand = new AsyncCommand(
                Sync,
                () => true);
        }

        private async Task Sync()
        {
            WriteToLog("Preparing...\n");
            await Task.Run(() => _task.Run());
            WriteToLog("Finished!\n");
        }

        private void WriteToLog(string line)
        {
            _log += line;
            OnPropertyChanged(nameof(Log));
        }

        private void Task_ProgressChanged(SyncNetBackupTask sender, SyncNetProgressChangedEventArgs args)
        {
            WriteToLog($"{DateTime.Now}: Uploaded {args.CurrentFile.Name}. {args.ProcessedFiles}/{args.TotalFiles} processed.\n");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
