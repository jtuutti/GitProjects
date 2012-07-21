using System;
using System.Globalization;
using RestFoundation.ServiceLocation;
using RestFoundation.StructureMap.Properties;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace RestFoundation.StructureMap
{
    internal sealed class ServiceBuilder
    {
        private readonly Registry m_registry;

        public ServiceBuilder(Registry registry)
        {
            if (registry == null) throw new ArgumentNullException("registry");

            m_registry = registry;
        }

        public void Build(bool mockContext)
        {
            var serviceContainer = new ServiceContainer(mockContext);

            foreach (var dependency in serviceContainer.SingletonServices)
            {
                RegisterDependency(dependency.Key, dependency.Value, true);
            }

            foreach (var dependency in serviceContainer.TransientServices)
            {
                RegisterDependency(dependency.Key, dependency.Value, false);
            }
        }

        private static ILifecycle GetLifecycle(bool isSingeton)
        {
            if (isSingeton)
            {
                return new SingletonLifecycle();
            }

            return new UniquePerRequestLifecycle();
        }

        private void RegisterDependency(Type abstractionType, Type implementationType, bool isSingleton)
        {
            if (abstractionType == null) throw new ArgumentNullException("abstractionType");
            if (implementationType == null) throw new ArgumentNullException("implementationType");

            if (!abstractionType.IsInterface && !abstractionType.IsAbstract)
            {
                throw new ArgumentException(Resources.InvalidAbstractionType, "abstractionType");
            }

            if (!implementationType.IsClass || implementationType.IsAbstract)
            {
                throw new ArgumentException(Resources.BadImplementationType, "implementationType");
            }

            try
            {
                m_registry.For(abstractionType).LifecycleIs(GetLifecycle(isSingleton)).Use(implementationType);
            }
            catch (Exception ex)
            {
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, Resources.DependencyRegistrationError, ex.Message), ex);
            }
        }
    }
}
