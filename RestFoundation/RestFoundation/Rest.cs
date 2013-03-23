// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Web.Routing;
using System.Web.Util;
using RestFoundation.Configuration;
using RestFoundation.Runtime;
using RestFoundation.Runtime.Handlers;
using RestFoundation.ServiceLocation;
using TinyIoC;

namespace RestFoundation
{
    /// <summary>
    /// Represents the REST Framework configuration class.
    /// </summary>
    public sealed class Rest : IDisposable
    {
        private static readonly IReadOnlyCollection<Type> serviceContextTypes = new[]
        {
            typeof(IServiceContext),
            typeof(IHttpRequest),
            typeof(IHttpResponse),
            typeof(IHttpResponseOutput),
            typeof(IServiceCache)
        };

        private static readonly Lazy<Rest> configurationBuilder = new Lazy<Rest>(() =>
        {
            RequestValidator.Current = new ServiceRequestValidator();
            return new Rest();
        }, true);

        private RestOptions m_options;

        private Rest()
        {
        }
        
        /// <summary>
        /// Gets the REST Foundation assembly instance for IoC support.
        /// </summary>
        public static Assembly FoundationAssembly
        {
            get
            {
                return typeof(Rest).Assembly;
            }
        }

        /// <summary>
        /// Gets a read-only collection of the service context dependent types.
        /// </summary>
        public static IReadOnlyCollection<Type> ServiceContextTypes
        {
            get
            {
                return serviceContextTypes;
            }
        }

        /// <summary>
        /// Gets the REST Foundation configuration instance.
        /// </summary>
        public static Rest Configuration
        {
            get
            {
                return configurationBuilder.Value;
            }
        }

        /// <summary>
        /// Gets the REST Foundation configuration options.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public RestOptions Options
        {
            get
            {
                if (m_options == null)
                {
                    throw new InvalidOperationException(Resources.Global.ConfigurationNotInitialized);
                }

                return m_options;
            }
        }

        /// <summary>
        /// Gets the service locator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; private set; }

        /// <summary>
        /// Initializes REST Foundation configuration with the default IoC container.
        /// </summary>
        /// <param name="serviceAssembly">The service assembly.</param>
        /// <returns>The configuration options object.</returns>
        public RestOptions Initialize(Assembly serviceAssembly)
        {
            return InitializeWithDefaultDependencies(serviceAssembly, false);
        }

        /// <summary>
        /// Initializes REST Foundation configuration with the default IoC container. Mocked
        /// service context is injected. Use this method only for unit testing.
        /// </summary>
        /// <param name="serviceAssembly">The service assembly.</param>
        /// <returns>The configuration options object.</returns>
        public RestOptions InitializeAndMock(Assembly serviceAssembly)
        {
            return InitializeWithDefaultDependencies(serviceAssembly, true);
        }

        /// <summary>
        /// Initializes REST Foundation configuration with the default IoC container.
        /// </summary>
        /// <param name="serviceAssemblyName">The service assembly name.</param>
        /// <returns>The configuration options object.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="serviceAssemblyName"/> is null.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException"><paramref name="serviceAssemblyName"/> is not found.</exception>
        /// <exception cref="T:System.BadImageFormatException"><paramref name="serviceAssemblyName"/> is not a valid assembly.</exception>
        /// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
        /// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
        public RestOptions Initialize(string serviceAssemblyName)
        {
            return InitializeWithDefaultDependencies(AppDomain.CurrentDomain.Load(serviceAssemblyName), false);
        }

        /// <summary>
        /// Initializes REST Foundation configuration with the default IoC container. Mocked
        /// service context is injected. Use this method only for unit testing.
        /// </summary>
        /// <param name="serviceAssemblyName">The service assembly name.</param>
        /// <returns>The configuration options object.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="serviceAssemblyName"/> is null.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException"><paramref name="serviceAssemblyName"/> is not found.</exception>
        /// <exception cref="T:System.BadImageFormatException"><paramref name="serviceAssemblyName"/> is not a valid assembly.</exception>
        /// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
        /// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
        public RestOptions InitializeAndMock(string serviceAssemblyName)
        {
            return InitializeWithDefaultDependencies(AppDomain.CurrentDomain.Load(serviceAssemblyName), true);
        }

        /// <summary>
        /// Initializes REST foundation configuration with custom dependencies.
        /// </summary>
        /// <param name="builder">The dependency builder.</param>
        /// <returns>The configuration options object.</returns>
        public RestOptions Initialize(Action<DependencyBuilder> builder)
        {
            return InitializeWithBuilder(builder, false);
        }

        /// <summary>
        /// Initializes REST foundation configuration with custom dependencies.
        /// </summary>
        /// <param name="builder">The dependency builder.</param>
        /// <returns>The configuration options object.</returns>
        public RestOptions InitializeAndMock(Action<DependencyBuilder> builder)
        {
            return InitializeWithBuilder(builder, true);
        }
        
        /// <summary>
        /// Initializes REST Foundation configuration with the default IoC container.
        /// </summary>
        /// <param name="serviceLocator">A service locator instance.</param>
        /// <returns>The configuration options object.</returns>
        public RestOptions Initialize(IServiceLocator serviceLocator)
        {
            if (serviceLocator == null)
            {
                throw new ArgumentNullException("serviceLocator");
            }

            if (ServiceLocator != null)
            {
                throw new InvalidOperationException(Resources.Global.AlreadyConfigured);
            }

            RouteCollection routes = RouteTable.Routes;
            routes.Add(new Route(String.Empty, serviceLocator.GetService<RootRouteHandler>()));

            ServiceLocator = serviceLocator;

            m_options = new RestOptions();
            return m_options;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (ServiceLocator != null)
            {
                ServiceLocator.Dispose();
            }
        }

        private static void RegisterDependencies(TinyIoCContainer container, bool mockContext, Func<Type, bool> registrationValidator)
        {
            var serviceContainer = new ServiceContainer(mockContext);

            foreach (var dependency in serviceContainer.SingletonServices)
            {
                if (registrationValidator != null && registrationValidator(dependency.Key))
                {
                    continue;
                }

                container.Register(dependency.Key, dependency.Value).AsSingleton();
            }

            foreach (var dependency in serviceContainer.TransientServices)
            {
                if (registrationValidator != null && registrationValidator(dependency.Key))
                {
                    continue;
                }

                container.Register(dependency.Key, dependency.Value).AsMultiInstance();
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
                         Justification = "This method is not responsible for disposing the IoC container")]
        private RestOptions InitializeWithDefaultDependencies(Assembly serviceAssembly, bool mockContext)
        {
            var container = new TinyIoCContainer();

            if (serviceAssembly != null)
            {
                container.AutoRegister(new[] { serviceAssembly });
            }

            RegisterDependencies(container, mockContext, null);

            return Initialize(new DefaultServiceLocator(container, null));
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
                         Justification = "This method is not responsible for disposing the IoC container")]
        private RestOptions InitializeWithBuilder(Action<DependencyBuilder> builder, bool mockContext)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            var container = new TinyIoCContainer();
            var dependencyBuilder = new DependencyBuilder(container);

            builder(dependencyBuilder);
            RegisterDependencies(container, mockContext, dependencyBuilder.IsRegistered);

            return Initialize(new DefaultServiceLocator(container, dependencyBuilder.PropertyInjectionPredicate));
        }
    }
}
