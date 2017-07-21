using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sync.Net.UI.Utils
{
    public class WindowManager : IWindowManager
    {
        public string ShowDirectoryDialog()
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
                return dialog.SelectedPath;
            return null;
        }

        public void ShutdownApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
