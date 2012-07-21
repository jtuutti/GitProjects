using System;
using Microsoft.Practices.Unity;
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
        /// Configures REST Foundation to use a Unity container injected with a mocked service context.
        /// </summary>
        /// <param name="restConfiguration">The REST configuration object.</param>
        /// <returns>The configuration object.</returns>
        public static Rest ConfigureMocksWithUnity(this Rest restConfiguration)
        {
            return RestConfigurator.Configure(null, true);
        }

        /// <summary>
        /// Configures REST Foundation to use a Unity container injected with a mocked service context.
        /// </summary>
        /// <param name="restConfiguration">The REST configuration object.</param>
        /// <param name="registrationBuilder">A delegate to specify additional service dependencies.</param>
        /// <returns>The configuration object.</returns>
        public static Rest ConfigureMocksWithUnity(this Rest restConfiguration, Action<IUnityContainer> registrationBuilder)
        {
            return RestConfigurator.Configure(registrationBuilder, true);
        }
    }
}
