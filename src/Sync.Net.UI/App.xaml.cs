using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
            base.OnStartup(e);
        }
    }
}
