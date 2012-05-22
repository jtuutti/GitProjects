using System;
using System.Linq;
using System.Reflection;
using MessageBus.Implementation;
using MessageBus.Msmq;
using StructureMap;

namespace MessageBus
{
    public static class BusConfigurationExtensions
    {
        public static IBusCreator UseStructureMap(this BusConfiguration configuration, string assemblyName)
        {
            if (String.IsNullOrEmpty(assemblyName)) throw new ArgumentNullException("assemblyName");

            return UseStructureMap(configuration, Assembly.Load(assemblyName), ObjectFactory.Container);
        }

        public static IBusCreator UseStructureMap(this BusConfiguration configuration, Assembly handlerAssembly)
        {
            return UseStructureMap(configuration, handlerAssembly, ObjectFactory.Container);
        }

        public static IBusCreator UseStructureMap(this BusConfiguration configuration, string assemblyName, IContainer container)
        {
            if (String.IsNullOrEmpty(assemblyName)) throw new ArgumentNullException("assemblyName");

            return UseStructureMap(configuration, Assembly.Load(assemblyName), container);
        }

        public static IBusCreator UseStructureMap(this BusConfiguration configuration, Assembly handlerAssembly, IContainer container)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (handlerAssembly == null) throw new ArgumentNullException("handlerAssembly");
            if (container == null) throw new ArgumentNullException("container");

            container.Configure(c =>
                                {
                                    c.For<IBus>().HybridHttpOrThreadLocalScoped().Use<MsmqBus>();
                                    c.Scan(a =>
                                           {
                                               a.Assembly(handlerAssembly);
                                               a.AddAllTypesOf<IMessageHandler>().NameBy(t => t.FullName);
                                           });
                                    c.SetAllProperties(a => a.Matching(p => p.PropertyType == typeof(IBus)));
                                });

            configuration.MessageCreator = container.GetInstance;
            configuration.RegisterHandlers(container.GetAllInstances<IMessageHandler>().ToArray());

            return new BusCreator(container);
        }
    }
}
