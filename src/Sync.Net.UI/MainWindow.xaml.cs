using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Sync.Net.Configuration;
using Sync.Net.UI.Utils;

namespace Sync.Net.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Sync_Click(object sender, RoutedEventArgs e)
        {
            var configuration = AppContainer.Container.Resolve<SyncNetConfiguration>();
            var factory = new SyncNetTaskFactory();
            var task = factory.Create(configuration);
            task.ProgressChanged += Task_ProgressChanged;
            textBox.AppendText("Preparing...\n");
            task.Backup();
            textBox.AppendText("Finished!\n");
        }

        private void Task_ProgressChanged(SyncNetBackupTask sender, SyncNetProgressChangedEventArgs e)
        {
            textBox.AppendText($"{DateTime.Now}: Uploaded {e.CurrentFile.Name}. {e.ProcessedFiles}/{e.TotalFiles} processed.\n");
        }
    }
}
