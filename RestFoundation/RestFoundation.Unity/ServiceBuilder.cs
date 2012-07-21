using System;
using System.Globalization;
using Microsoft.Practices.Unity;
using RestFoundation.ServiceLocation;
using RestFoundation.Unity.Properties;

namespace RestFoundation.Unity
{
    internal sealed class ServiceBuilder
    {
        private readonly IUnityContainer m_container;

        public ServiceBuilder(IUnityContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");

            m_container = container;
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

        private static LifetimeManager GetLifetimeManager(bool isSingleton)
        {
            return isSingleton ? (LifetimeManager) new ContainerControlledLifetimeManager() : new PerResolveLifetimeManager();
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
                m_container.RegisterType(abstractionType, implementationType, GetLifetimeManager(isSingleton));
            }
            catch (Exception ex)
            {
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, Resources.DependencyRegistrationError, ex.Message), ex);
            }
        }
    }

}
