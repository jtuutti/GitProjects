// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Practices.Unity;
using RestFoundation.ServiceLocation;

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

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
                         Justification = "The lifetime manager will be disposed by the container.")]
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
                throw new ArgumentException(Properties.Resources.InvalidAbstractionType, "abstractionType");
            }

            if (!implementationType.IsClass || implementationType.IsAbstract)
            {
                throw new ArgumentException(Properties.Resources.BadImplementationType, "implementationType");
            }

            if (m_container.IsRegistered(abstractionType))
            {
                return;
            }

            try
            {
                m_container.RegisterType(abstractionType, implementationType, GetLifetimeManager(isSingleton));
            }
            catch (Exception ex)
            {
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, Properties.Resources.DependencyRegistrationError, ex.Message), ex);
            }
        }
    }

}
