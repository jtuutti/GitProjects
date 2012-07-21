using System;
using System.Globalization;
using Microsoft.Practices.Unity;
using RestFoundation.DependencyInjection;
using RestFoundation.Unity.Properties;

namespace RestFoundation.Unity
{
    public sealed class DependencyRegistry : IDependencyRegistry, IDisposable
    {
        private readonly IUnityContainer m_container;

        public DependencyRegistry(IUnityContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");

            m_container = container;
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
                m_container.RegisterType(abstractionType, implementationType, key, GetLifetimeManager(lifetime));
            }
            catch (Exception ex)
            {
                throw new DependencyInjectionException(String.Format(CultureInfo.InvariantCulture, Resources.DependencyRegistrationError, ex.Message), ex);
            }
        }

        private static LifetimeManager GetLifetimeManager(DependencyLifetime lifetime)
        {
            return lifetime == DependencyLifetime.Singleton ? (LifetimeManager) new ContainerControlledLifetimeManager() : new PerResolveLifetimeManager();
        }

        public void Dispose()
        {
            m_container.Dispose();
        }
    }
}
