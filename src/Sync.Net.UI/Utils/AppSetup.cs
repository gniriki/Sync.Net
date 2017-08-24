using System.Reflection;
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

            using (var stream = new ConfigFile().GetStream())
            {
                var configuration = SyncNetConfiguration.Load(stream);
                cb.RegisterInstance(configuration).As<SyncNetConfiguration>();

                var syncNetFactory = new SyncNetTaskFactory();
                var task = syncNetFactory.Create(configuration);

                cb.RegisterInstance(task).As<ISyncNetTask>();
            }

            ILogger logger = new Logger();
            cb.RegisterInstance(logger).As<ILogger>();
        }
    }
}