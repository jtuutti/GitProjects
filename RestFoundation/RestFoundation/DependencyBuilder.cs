// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using RestFoundation.ServiceLocation;
using TinyIoC;

namespace RestFoundation
{
    /// <summary>
    /// Represents a dependency builder for the default service locator.
    /// </summary>
    public sealed class DependencyBuilder
    {
        private readonly TinyIoCContainer m_container;
        private readonly HashSet<Type> m_registeredTypes;

        internal DependencyBuilder(TinyIoCContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            m_container = container;
            m_registeredTypes = new HashSet<Type>();
        }

        internal Func<Type, bool> PropertyInjectionPredicate { get; private set; }

        /// <summary>
        /// Enables property injection for the types that match the predicate. The predicate should include
        /// contract and implementation classes.
        /// </summary>
        /// <remarks>
        /// Use the <code>type => true</code> delegate to allow property injection to all types. This is not
        /// recommended since it can make the classes brittle if new public properties are defined.
        /// </remarks>
        /// <param name="typePredicate">The type predicate.</param>
        /// <returns>The dependency builder.</returns>
        public DependencyBuilder AllowPropertyInjection(Func<Type, bool> typePredicate)
        {
            if (typePredicate == null)
            {
                throw new ArgumentNullException("typePredicate");
            }

            PropertyInjectionPredicate = typePredicate;
            return this;
        }

        /// <summary>
        /// Registers a service implementation for its contract with the per-instance lifetime.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <typeparam name="TImplementation">The service implementation type.</typeparam>
        /// <returns>The dependency builder.</returns>
        public DependencyBuilder Register<TContract, TImplementation>()
            where TContract : class
            where TImplementation : class, TContract
        {
            return Register<TContract, TImplementation>(InstanceLifetime.PerInstance);
        }

        /// <summary>
        /// Registers a service implementation for its contract with the provided lifetime.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <typeparam name="TImplementation">The service implementation type.</typeparam>
        /// <param name="lifetime">The instance lifetime.</param>
        /// <returns>The dependency builder.</returns>
        public DependencyBuilder Register<TContract, TImplementation>(InstanceLifetime lifetime)
            where TContract : class
            where TImplementation : class, TContract
        {
            try
            {
                TinyIoCContainer.RegisterOptions options = m_container.Register<TContract, TImplementation>();
                AssignLifetime(options, lifetime);
            }
            catch (Exception ex)
            {
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, RestResources.DependencyRegistrationError, ex.Message), ex);
            }

            m_registeredTypes.Add(typeof(TContract));

            return this;
        }

        /// <summary>
        /// Registers a service implementation for its contract with the per-instance lifetime.
        /// </summary>
        /// <param name="contractType">The service contract type.</param>
        /// <param name="implementationType">The service implementation type.</param>
        /// <returns>The dependency builder.</returns>
        public DependencyBuilder Register(Type contractType, Type implementationType)
        {
            return Register(contractType, implementationType, InstanceLifetime.PerInstance);
        }

        /// <summary>
        /// Registers a service implementation for its contract with the provided lifetime.
        /// </summary>
        /// <param name="contractType">The service contract type.</param>
        /// <param name="implementationType">The service implementation type.</param>
        /// <param name="lifetime">The instance lifetime.</param>
        /// <returns>The dependency builder.</returns>
        public DependencyBuilder Register(Type contractType, Type implementationType, InstanceLifetime lifetime)
        {
            if (contractType == null)
            {
                throw new ArgumentNullException("contractType");
            }

            if (implementationType == null)
            {
                throw new ArgumentNullException("implementationType");
            }

            try
            {
                TinyIoCContainer.RegisterOptions options = m_container.Register(contractType, implementationType);
                AssignLifetime(options, lifetime);
            }
            catch (Exception ex)
            {
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, RestResources.DependencyRegistrationError, ex.Message), ex);
            }

            m_registeredTypes.Add(contractType);

            return this;
        }

        /// <summary>
        /// Registers a specific service implementation instance to its contract with the per-instance lifetime.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="instance">The service instance.</param>
        /// <returns>The dependency builder.</returns>
        public DependencyBuilder Register<TContract>(TContract instance)
            where TContract : class
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            try
            {
                m_container.Register(instance);
            }
            catch (Exception ex)
            {
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, RestResources.DependencyRegistrationError, ex.Message), ex);
            }

            m_registeredTypes.Add(typeof(TContract));

            return this;
        }

        /// <summary>
        /// Registers a specific service implementation instance to its contract with the per-instance lifetime.
        /// </summary>
        /// <param name="contractType">The service contract type.</param>
        /// <param name="instance">The service instance.</param>
        /// <returns>The dependency builder.</returns>
        public DependencyBuilder Register(Type contractType, object instance)
        {
            if (contractType == null)
            {
                throw new ArgumentNullException("contractType");
            }

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            try
            {
                m_container.Register(contractType, instance);
            }
            catch (Exception ex)
            {
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, RestResources.DependencyRegistrationError, ex.Message), ex);
            }

            m_registeredTypes.Add(contractType);

            return this;
        }

        /// <summary>
        /// Scans assemblies with the provided names and registers the inner service implementations to their contracts.
        /// </summary>
        /// <param name="assemblyNames">An array of assembly names to scan.</param>
        /// <returns>The dependency builder.</returns>
        public DependencyBuilder ScanAssemblies(params string[] assemblyNames)
        {
            return ScanAssemblies(assemblyNames, null);
        }

        /// <summary>
        /// Scans assemblies with the provided names and registers the inner service implementations to their contracts.
        /// The registration predicate must account for both, service contracts and implementations.
        /// </summary>
        /// <param name="assemblyNames">An array of assembly names to scan.</param>
        /// <param name="registrationPredicate">The registration predicate.</param>
        /// <returns>The dependency builder.</returns>
        public DependencyBuilder ScanAssemblies(string[] assemblyNames, Func<Type, bool> registrationPredicate)
        {
            if (assemblyNames == null)
            {
                throw new ArgumentNullException("assemblyNames");
            }

            if (assemblyNames.Length == 0)
            {
                return this;
            }

            var assemblies = new List<Assembly>();

            try
            {
                foreach (string assemblyName in assemblyNames)
                {
                    assemblies.Add(AppDomain.CurrentDomain.Load(assemblyName));
                }
            }
            catch (Exception ex)
            {
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, RestResources.DependencyRegistrationError, ex.Message), ex);
            }

            return ScanAssemblies(assemblies.ToArray(), registrationPredicate);
        }

        /// <summary>
        /// Scans the provided assemblies and registers the inner service implementations to their contracts.
        /// </summary>
        /// <param name="assemblies">An array of assemblies to scan.</param>
        /// <returns>The dependency builder.</returns>
        public DependencyBuilder ScanAssemblies(params Assembly[] assemblies)
        {
            return ScanAssemblies(assemblies, null);
        }

        /// <summary>
        /// Scans the provided assemblies and registers the inner service implementations to their contracts.
        /// The registration predicate must account for both, service contracts and implementations.
        /// </summary>
        /// <param name="assemblies">An array of assemblies to scan.</param>
        /// <param name="registrationPredicate">The registration predicate.</param>
        /// <returns>The dependency builder.</returns>
        public DependencyBuilder ScanAssemblies(Assembly[] assemblies, Func<Type, bool> registrationPredicate)
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException("assemblies");
            }

            if (assemblies.Length == 0)
            {
                return this;
            }

            try
            {
                if (registrationPredicate != null)
                {
                    m_container.AutoRegister(assemblies, registrationPredicate);
                }
                else
                {
                    m_container.AutoRegister(assemblies);
                }
            }
            catch (Exception ex)
            {
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, RestResources.DependencyRegistrationError, ex.Message), ex);
            }

            return this;
        }

        internal bool IsRegistered(Type dependencyType)
        {
            if (dependencyType == null)
            {
                throw new ArgumentNullException("dependencyType");
            }

            return m_registeredTypes.Contains(dependencyType);
        }

        private static void AssignLifetime(TinyIoCContainer.RegisterOptions options, InstanceLifetime lifetime)
        {
            switch (lifetime)
            {
                case InstanceLifetime.Singleton:
                    options.AsSingleton();
                    break;
                case InstanceLifetime.PerHttpContext:
                    options.AsPerRequestSingleton();
                    break;
                default:
                    options.AsMultiInstance();
                    break;
            }
        }
    }
}
