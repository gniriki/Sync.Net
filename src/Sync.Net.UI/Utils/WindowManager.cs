using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
