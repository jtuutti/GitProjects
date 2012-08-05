using System;
using System.Diagnostics.CodeAnalysis;
using RestFoundation.ServiceLocation;
using RestFoundation.StructureMap;
using StructureMap;

namespace RestFoundation.UnitTesting
{
    /// <summary>
    /// Defines extensions for a <see cref="Rest"/> object to configure REST Foundation for unit testing
    /// to use a StructureMap container.
    /// </summary>
    public static class MockExtensions
    {
        /// <summary>
        /// Configures REST Foundation with the provided StructureMap container and configures it with mocked service context.
        /// This method is used for unit testing.
        /// </summary>
        /// <param name="restConfiguration">The REST configuration object.</param>
        /// <param name="container">The StructureMap container.</param>
        /// <returns>The configuration object.</returns>
        public static Rest ConfigureMocksWithStructureMap(this Rest restConfiguration, IContainer container)
        {
            if (restConfiguration == null)
            {
                throw new ArgumentNullException("restConfiguration");
            }

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
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
                         Justification = "The IoC container will be disposed by the unit test through disposing the service locator.")]
        public static IServiceLocator CreateServiceLocatorForStructureMap(this Rest restConfiguration, bool mockContext)
        {
            if (restConfiguration == null)
            {
                throw new ArgumentNullException("restConfiguration");
            }

            return RestConfigurator.CreateServiceLocator(new Container(), mockContext);
        }

        /// <summary>
        /// Creates a new instance of the service locator with the provided StructureMap container. This method is used for unit testing.
        /// </summary>
        /// <param name="restConfiguration">The REST configuration object.</param>
        /// <param name="container">The StructureMap container.</param>
        /// <param name="mockContext">A value indicating whether to inject mocked service context.</param>
        /// <returns>The service locator.</returns>
        public static IServiceLocator CreateServiceLocatorForStructureMap(this Rest restConfiguration, IContainer container, bool mockContext)
        {
            if (restConfiguration == null)
            {
                throw new ArgumentNullException("restConfiguration");
            }

            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            return RestConfigurator.CreateServiceLocator(container, mockContext);
        }
    }
}
