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
        private ConfigFile _configFile = new ConfigFile();

        protected override void OnStartup(StartupEventArgs e)
        {
            AppContainer.Container = new AppSetup().CreateContainer();
            StaticLogger.Logger = AppContainer.Container.Resolve<ILogger>();

            var configurationProvider = AppContainer.Container.Resolve<IConfigurationProvider>();
            var windowManager = AppContainer.Container.Resolve<IWindowManager>();

            if (_configFile.Exists())
            {
                StartProcessor(configurationProvider.Current);
                ShowMainWindow();
                CreateTaskbarIcon();
            }
            else
            {
                App.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;
                configurationProvider.Create();
                windowManager.ShowConfiguration();
            }

            base.OnStartup(e);
        }

        private static void StartProcessor(SyncNetConfiguration configuration)
        {
            Task.Run(() =>
            {
                var syncNetFactory = new SyncNetProcessorFactory();
                var processor = syncNetFactory.Create(configuration);

                StaticLogger.Log("Updating files...");
                processor.ProcessSourceDirectoryAsync();

                StaticLogger.Log("Starting file watcher...");
                var watcher = new SyncNetWatcher(processor,
                    AppContainer.Container.Resolve<IConfigurationProvider>(),
                    AppContainer.Container.Resolve<IFileWatcher>());
                watcher.Watch();

                StaticLogger.Log("Ready.");
            });
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