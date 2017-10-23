using System.Reflection;
using System.Windows;
using Autofac;
using Sync.Net.Configuration;

namespace Sync.Net.UI.Utils
{
    internal class AppSetup
    {
        public IContainer CreateContainer(SyncNetConfiguration configuration)
        {
            var containerBuilder = new ContainerBuilder();
            RegisterDependencies(containerBuilder, configuration);
            return containerBuilder.Build();
        }

        protected virtual void RegisterDependencies(ContainerBuilder cb, SyncNetConfiguration configuration)
        {
            var assembly = Assembly.GetExecutingAssembly();

            cb.RegisterAssemblyTypes(assembly)
                .Where(x => x.Name.EndsWith("ViewModel"))
                .SingleInstance();

            cb.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();

            cb.RegisterType<SyncNetWatcher>();

            cb.RegisterType<ConfigurationTester>().As<IConfigurationTester>();

            cb.RegisterInstance(configuration).As<SyncNetConfiguration>();

            var syncNetFactory = new SyncNetTaskFactory();
            var task = syncNetFactory.Create(configuration);
            cb.RegisterInstance(task).As<ISyncNetTask>();

            ILogger logger = new Logger();
            cb.RegisterInstance(logger).As<ILogger>();
        }
    }
}