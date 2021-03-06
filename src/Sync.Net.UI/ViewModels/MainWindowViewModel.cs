﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Sync.Net.UI.Annotations;
using Sync.Net.UI.Utils;

namespace Sync.Net.UI.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ILogger _logger;

        public MainWindowViewModel(IWindowManager windowManager,
            ILogger logger)
        {
            _logger = logger;
            _logger.LogUpdated += _logger_LogUpdated;
            Log = logger.Contents;

            ExitCommand = new RelayCommand(
                p => true,
                p => { windowManager.ShutdownApplication(); });

            ConfigureCommand = new RelayCommand(
                p => true,
                p =>
                {
                    windowManager.ShowConfiguration();
                });
        }

        public ICommand ExitCommand { get; }
        public ICommand ConfigureCommand { get; }

        public string Log { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void _logger_LogUpdated(string newLine)
        {
            Log += newLine;
            OnPropertyChanged(nameof(Log));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}