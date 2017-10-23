using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Hardcodet.Wpf.TaskbarNotification;
using Sync.Net.Configuration;
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
            SyncNetConfiguration configuration = LoadConfiguration();
            AppContainer.Container = new AppSetup().CreateContainer(configuration);
            StaticLogger.Logger = AppContainer.Container.Resolve<ILogger>();

            CreateTaskbarIcon();

            var task = AppContainer.Container.Resolve<ISyncNetTask>();
            Task.Run(() =>
            {
                StaticLogger.Log("Updating files...");
                task.ProcessSourceDirectoryAsync();

                StaticLogger.Log("Starting file watcher...");
                var watcher = AppContainer.Container.Resolve<SyncNetWatcher>();
                watcher.Watch();

                StaticLogger.Log("Ready.");
            });

            base.OnStartup(e);
        }

        private static SyncNetConfiguration LoadConfiguration()
        {
            SyncNetConfiguration configuration;
            using (var stream = new ConfigFile().GetStream())
            {
                configuration = SyncNetConfiguration.Load(stream);
            }
            return configuration;
        }

        private void CreateTaskbarIcon()
        {
            StaticLogger.Log("Creating taskbar icon...");

            var tbi = new TaskbarIcon();
            using (var iconStream = GetResourceStream(new Uri("pack://application:,,,/TrayIcon.ico")).Stream)
            {
                tbi.Icon = new Icon(iconStream);
            }
            tbi.ToolTipText = "Sync.Net";

            tbi.DoubleClickCommand = new RelayCommand(p => true, p => ShowMainWindow());
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