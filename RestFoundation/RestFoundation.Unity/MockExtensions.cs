using System;
using Microsoft.Practices.Unity;
using RestFoundation.ServiceLocation;
using RestFoundation.Unity;

namespace RestFoundation.UnitTesting
{
    /// <summary>
    /// Defines extensions for a <see cref="Rest"/> object to configure REST Foundation for unit testing
    /// to use a Unity container.
    /// </summary>
    public static class MockExtensions
    {
        /// <summary>
        /// Configures REST Foundation with the provided Unity container and configures it with mocked service context.
        /// This method is used for unit testing.
        /// </summary>
        /// <param name="restConfiguration">The REST configuration object.</param>
        /// <param name="container">The Unity container.</param>
        /// <returns>The configuration object.</returns>
        public static Rest ConfigureMocksWithUnity(this Rest restConfiguration, IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            return RestConfigurator.Configure(container, true);
        }

        /// <summary>
        /// Creates a new instance of the service locator with a new container. This method is used for unit testing.
        /// </summary>
        /// <param name="restConfiguration">The REST configuration object.</param>
        /// <param name="mockContext">A value indicating whether to inject mocked service context.</param>
        /// <returns>The service locator.</returns>
        public static IServiceLocator CreateServiceLocatorForUnity(this Rest restConfiguration, bool mockContext)
        {
            return RestConfigurator.CreateServiceLocator(new UnityContainer(), mockContext);
        }

        /// <summary>
        /// Creates a new instance of the service locator with the provided Unity container. This method is used for unit testing.
        /// </summary>
        /// <param name="restConfiguration">The REST configuration object.</param>
        /// <param name="container">The Unity container.</param>
        /// <param name="mockContext">A value indicating whether to inject mocked service context.</param>
        /// <returns>The service locator.</returns>
        public static IServiceLocator CreateServiceLocatorForUnity(this Rest restConfiguration, IUnityContainer container, bool mockContext)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            return RestConfigurator.CreateServiceLocator(container, mockContext);
        }
    }
}
