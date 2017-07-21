using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autofac;
using Hardcodet.Wpf.TaskbarNotification;
using Sync.Net.Configuration;
using Sync.Net.UI.Utils;

namespace Sync.Net.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SynchronizationContext synchronizationContext;

        public MainWindow()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
        }

        private async void Sync_Click(object sender, RoutedEventArgs e)
        {
            var configuration = AppContainer.Container.Resolve<SyncNetConfiguration>();
            var factory = new SyncNetTaskFactory();
            var task = factory.Create(configuration);
            task.ProgressChanged += Task_ProgressChanged;
            textBox.AppendText("Preparing...\n");
            await Task.Run(() => task.Backup());
            textBox.AppendText("Finished!\n");
        }

        private void Task_ProgressChanged(SyncNetBackupTask sender, SyncNetProgressChangedEventArgs e)
        {
            synchronizationContext.Post(o =>
            {
                var args = (SyncNetProgressChangedEventArgs) o;
                textBox.AppendText(
                    $"{DateTime.Now}: Uploaded {args.CurrentFile.Name}. {args.ProcessedFiles}/{args.TotalFiles} processed.\n");
            }, e);
        }
    }
}
