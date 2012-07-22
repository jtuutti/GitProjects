using System;
using RestFoundation.ServiceLocation;
using RestFoundation.StructureMap;
using StructureMap.Configuration.DSL;

namespace RestFoundation.UnitTesting
{
    /// <summary>
    /// Defines extensions for a <see cref="Rest"/> object to configure REST Foundation for unit testing
    /// to use a StructureMap container.
    /// </summary>
    public static class MockExtensions
    {
        /// <summary>
        /// Configures REST Foundation to use a StructureMap container injected with a mocked service context.
        /// </summary>
        /// <param name="restConfiguration">The REST configuration object.</param>
        /// <returns>The configuration object.</returns>
        public static Rest ConfigureMocksWithStructureMap(this Rest restConfiguration)
        {
            return RestConfigurator.Configure(null, true);
        }

        /// <summary>
        /// Configures REST Foundation to use a StructureMap container injected with a mocked service context.
        /// </summary>
        /// <param name="restConfiguration">The REST configuration object.</param>
        /// <param name="registrationBuilder">A delegate to specify additional service dependencies.</param>
        /// <returns>The configuration object.</returns>
        public static Rest ConfigureMocksWithStructureMap(this Rest restConfiguration, Action<Registry> registrationBuilder)
        {
            return RestConfigurator.Configure(registrationBuilder, true);
        }

        /// <summary>
        /// Creates a new instance of a service locator for a StructureMap container. This method is used for unit testing.
        /// </summary>
        /// <param name="restConfiguration">The REST configuration object.</param>
        /// <param name="mockContext">A value indicating whether to inject mocked service context.</param>
        /// <returns>The service locator.</returns>
        public static IServiceLocator CreateServiceLocatorForStructureMap(this Rest restConfiguration, bool mockContext)
        {
            return RestConfigurator.CreateServiceLocator(null, mockContext);
        }

        /// <summary>
        /// Creates a new instance of a service locator for a StructureMap container. This method is used for unit testing.
        /// </summary>
        /// <param name="restConfiguration">The REST configuration object.</param>
        /// <param name="registrationBuilder">A delegate to specify additional service dependencies.</param>
        /// <param name="mockContext">A value indicating whether to inject mocked service context.</param>
        /// <returns>The service locator.</returns>
        public static IServiceLocator CreateServiceLocatorForStructureMap(this Rest restConfiguration, Action<Registry> registrationBuilder, bool mockContext)
        {
            return RestConfigurator.CreateServiceLocator(registrationBuilder, mockContext);
        }
    }
}
