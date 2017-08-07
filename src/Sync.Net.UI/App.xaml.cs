using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Hardcodet.Wpf.TaskbarNotification;
using Sync.Net.UI.Utils;

namespace Sync.Net.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppContainer.Container = new AppSetup().CreateContainer();

            StaticLogger.Logger = AppContainer.Container.Resolve<ILogger>();

            StaticLogger.Log("Creating taskbar icon...");

            TaskbarIcon tbi = new TaskbarIcon();
            using (Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/TrayIcon.ico")).Stream)
                tbi.Icon = new System.Drawing.Icon(iconStream);
            tbi.ToolTipText = "Sync.Net";
            tbi.DoubleClickCommand = new RelayCommand(p => true, p=> ShowMainWindow());

            StaticLogger.Log("Starting file watcher...");

            var watcher = AppContainer.Container.Resolve<SyncNetWatcher>();
            watcher.Watch();

            StaticLogger.Log("Ready.");
            base.OnStartup(e);
        }

        private void ShowMainWindow()
        {
            if (MainWindow == null)
            {
                MainWindow = new MainWindow();
            }

            MainWindow.Visibility = Visibility.Visible;
            MainWindow.Activate();
        }
    }
}
