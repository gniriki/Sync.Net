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
        public ICommand ExitCommand { get; }
        public ICommand SyncCommand { get; }

        public string Log => _log;

        public MainWindowViewModel(IWindowManager windowManager, 
            ISyncNetTaskFactory taskFactory,
            SyncNetConfiguration configuration)
        {
            _windowManager = windowManager;
            _taskFactory = taskFactory;
            _log = string.Empty;
            ExitCommand = new RelayCommand(
                p => true,
                p =>
                {
                    _windowManager.ShutdownApplication();
                });

            SyncCommand = new RelayCommand(
                p => true,
                async p =>
                {
                    var task = _taskFactory.Create(configuration);
                    task.ProgressChanged += Task_ProgressChanged;
                    WriteToLog("Preparing...\n");
                    await Task.Run(() => task.Run());
                    WriteToLog("Finished!\n");
                });
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
