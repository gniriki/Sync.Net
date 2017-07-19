using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Sync.Net.UI.Utils;

namespace Sync.Net.UI.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {

        }

        public ConfigurationViewModel Configuration => AppContainer.Container.Resolve<ConfigurationViewModel>();

        public static void Cleanup()
        {
        }
    }
}
