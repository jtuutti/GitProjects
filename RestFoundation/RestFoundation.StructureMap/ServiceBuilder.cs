﻿// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Globalization;
using RestFoundation.ServiceLocation;
using StructureMap;
using StructureMap.Pipeline;

namespace RestFoundation.StructureMap
{
    internal sealed class ServiceBuilder
    {
        private readonly IContainer m_container;

        public ServiceBuilder(IContainer container)
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
                m_container.Configure(configure => configure.For(abstractionType).LifecycleIs(GetLifecycle(isSingleton)).Use(implementationType));
            }
            catch (Exception ex)
            {
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, Properties.Resources.DependencyRegistrationError, ex.Message), ex);
            }
        }
    }
}
