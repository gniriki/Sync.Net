using System.Windows;
using Ookii.Dialogs.Wpf;

namespace Sync.Net.UI.Utils
{
    public class WindowManager : IWindowManager
    {
        public string ShowDirectoryDialog()
        {
            var dialog = new VistaFolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
                return dialog.SelectedPath;
            return null;
        }

        public void ShutdownApplication()
        {
            Application.Current.Shutdown();
        }

        public void RestartApplication()
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public void ShowConfiguration()
        {
            var window = new Window {Content = new Configuration()};
            window.Show();
        }
    }
}