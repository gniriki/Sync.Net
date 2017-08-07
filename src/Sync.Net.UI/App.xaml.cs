using System;
using System.Drawing;
using System.Windows;
using Autofac;
using Hardcodet.Wpf.TaskbarNotification;
using Sync.Net.UI.Utils;

namespace Sync.Net.UI
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppContainer.Container = new AppSetup().CreateContainer();

            StaticLogger.Logger = AppContainer.Container.Resolve<ILogger>();

            StaticLogger.Log("Creating taskbar icon...");

            var tbi = new TaskbarIcon();
            using (var iconStream = GetResourceStream(new Uri("pack://application:,,,/TrayIcon.ico")).Stream)
            {
                tbi.Icon = new Icon(iconStream);
            }
            tbi.ToolTipText = "Sync.Net";
            tbi.DoubleClickCommand = new RelayCommand(p => true, p => ShowMainWindow());

            StaticLogger.Log("Starting file watcher...");

            var watcher = AppContainer.Container.Resolve<SyncNetWatcher>();
            watcher.Watch();

            StaticLogger.Log("Ready.");
            base.OnStartup(e);
        }

        private void ShowMainWindow()
        {
            if (MainWindow == null)
                MainWindow = new MainWindow();

            MainWindow.Visibility = Visibility.Visible;
            MainWindow.Activate();
        }
    }
}