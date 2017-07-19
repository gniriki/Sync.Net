﻿using System.Reflection;
using Autofac;
using Sync.Net.Configuration;

namespace Sync.Net.UI.Utils
{
    class AppSetup
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

            cb.RegisterType<SyncNetConfiguration>().SingleInstance();
        }
    }
}
