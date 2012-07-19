using System;
using System.Globalization;
using RestFoundation.DependencyInjection;
using RestFoundation.StructureMap.Properties;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace RestFoundation.StructureMap
{
    public sealed class DependencyRegistry : IDependencyRegistry
    {
        private readonly Registry m_registry;

        public DependencyRegistry(Registry registry)
        {
            if (registry == null) throw new ArgumentNullException("registry");

            m_registry = registry;
        }

        public void Register(Type abstractionType, Type implementationType, DependencyLifetime lifetime, string key)
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
                ConfiguredInstance instance = m_registry.For(abstractionType).LifecycleIs(GetLifecycle(lifetime)).Use(implementationType);

                if (!String.IsNullOrEmpty(key))
                {
                    instance.Named(key);
                }
            }
            catch (Exception ex)
            {
                throw new DependencyInjectionException(String.Format(CultureInfo.InvariantCulture, Resources.DependencyRegistrationError, ex.Message), ex);
            }
        }

        private static ILifecycle GetLifecycle(DependencyLifetime lifetime)
        {
            return lifetime == DependencyLifetime.Singleton ? (ILifecycle) new SingletonLifecycle() : new UniquePerRequestLifecycle();
        }
    }
}
