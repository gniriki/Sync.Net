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

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }
    }
}