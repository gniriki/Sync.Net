using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Sync.Net.Configuration;
using Sync.Net.UI.Annotations;
using Sync.Net.UI.Utils;

namespace Sync.Net.UI.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ILogger _logger;
        private readonly ISyncNetTask _task;

        public MainWindowViewModel(IWindowManager windowManager,
            ISyncNetTask task,
            SyncNetConfiguration configuration,
            ILogger logger)
        {
            _logger = logger;
            _logger.LogUpdated += _logger_LogUpdated;
            Log = logger.Contents;
            _task = task;

            ExitCommand = new RelayCommand(
                p => true,
                p => { windowManager.ShutdownApplication(); });

            SyncCommand = new AsyncCommand(Sync,
                () => true);
        }

        public ICommand ExitCommand { get; }
        public AsyncCommand SyncCommand { get; }

        public string Log { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void _logger_LogUpdated(string newLine)
        {
            Log += newLine;
            OnPropertyChanged(nameof(Log));
        }

        public async Task Sync()
        {
            await Task.Run(() => _task.ProcessFiles());
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}