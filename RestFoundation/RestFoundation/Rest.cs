// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Web.Routing;
using System.Web.Util;
using RestFoundation.Runtime;
using RestFoundation.Runtime.Handlers;
using RestFoundation.ServiceLocation;
using TinyIoC;

namespace RestFoundation
{
    /// <summary>
    /// Represents the REST Framework configuration class.
    /// </summary>
    public sealed class Rest
    {
        private static readonly object syncRoot = new object();
        private static readonly ICollection<Type> serviceContextTypes = new ReadOnlyCollection<Type>(new[]
        {
            typeof(IServiceContext),
            typeof(IHttpRequest),
            typeof(IHttpResponse),
            typeof(IHttpResponseOutput),
            typeof(IServiceCache)
        });

        private static Rest configuration;
        private RestOptions options;

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
        /// Gets the service context dependent types.
        /// </summary>
        public static ICollection<Type> ServiceContextTypes
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
                if (configuration != null)
                {
                    return configuration;
                }

                lock (syncRoot)
                {
                    RequestValidator.Current = new ServiceRequestValidator();

                    return configuration ?? (configuration = new Rest());
                }
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
                if (options == null)
                {
                    throw new InvalidOperationException(RestResources.ConfigurationNotInitialized);
                }

                return options;
            }
        }

        /// <summary>
        /// Gets the service locator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; private set; }

        /// <summary>
        /// Initializes REST Foundation configuration with the default IoC container.
        /// </summary>
        /// <returns>The configuration options object.</returns>
        public RestOptions Initialize()
        {
            return Initialize(false);
        }

        /// <summary>
        /// Initializes REST Foundation configuration with the default IoC container. Mocked
        /// service context is injected. Use this method only for unit testing.
        /// </summary>
        /// <returns>The configuration options object.</returns>
        public RestOptions InitializeAndMock()
        {
            return Initialize(true);
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
                throw new InvalidOperationException(RestResources.AlreadyConfigured);
            }

            RouteCollection routes = RouteTable.Routes;
            routes.Add(new Route(String.Empty, serviceLocator.GetService<RootRouteHandler>()));

            ServiceLocator = serviceLocator;

            options = new RestOptions();
            return options;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
                         Justification = "This method is not responsible for disposing the IoC container")]
        private RestOptions Initialize(bool mockContext)
        {
            var defaultIoCContainer = new TinyIoCContainer();

            var serviceContainer = new ServiceContainer(mockContext);

            foreach (var dependency in serviceContainer.SingletonServices)
            {
                defaultIoCContainer.Register(dependency.Key, dependency.Value).AsSingleton();
            }

            foreach (var dependency in serviceContainer.TransientServices)
            {
                defaultIoCContainer.Register(dependency.Key, dependency.Value).AsMultiInstance();
            }

            return Initialize(new DefaultServiceLocator(defaultIoCContainer));
        }
    }
}
