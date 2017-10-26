using System.Reflection;
using System.Windows;
using Autofac;
using Sync.Net.Configuration;

namespace Sync.Net.UI.Utils
{
    internal class AppSetup
    {
        public IContainer CreateContainer()
        {
            var containerBuilder = new ContainerBuilder();
            RegisterDependencies(containerBuilder);
            return containerBuilder.Build();
        }

        protected virtual void RegisterDependencies(ContainerBuilder cb)
        {
            var assembly = Assembly.GetExecutingAssembly();

            cb.RegisterAssemblyTypes(assembly)
                .Where(x => x.Name.EndsWith("ViewModel"))
                .SingleInstance();

            cb.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();

            cb.RegisterType<SyncNetWatcher>();

            cb.RegisterType<ConfigurationTester>().As<IConfigurationTester>();

            cb.RegisterType<ConfigurationProvider>().As<IConfigurationProvider>();

            ILogger logger = new Logger();
            cb.RegisterInstance(logger).As<ILogger>();
        }
    }
}