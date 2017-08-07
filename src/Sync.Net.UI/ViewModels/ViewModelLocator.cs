using Autofac;
using Sync.Net.UI.Utils;

namespace Sync.Net.UI.ViewModels
{
    public class ViewModelLocator
    {
        public ConfigurationViewModel Configuration => AppContainer.Container.Resolve<ConfigurationViewModel>();
        public MainWindowViewModel MainWindow => AppContainer.Container.Resolve<MainWindowViewModel>();

        public static void Cleanup()
        {
        }
    }
}