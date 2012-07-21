using System;
using Microsoft.Practices.Unity;
using RestFoundation.Unity;

namespace RestFoundation
{
    /// <summary>
    /// Defines extensions for a <see cref="Rest"/> object to configure REST Foundation to use a Unity container.
    /// </summary>
    public static class RestExtensions
    {
        /// <summary>
        /// Configures REST Foundation to use a Unity container.
        /// </summary>
        /// <param name="restConfiguration">The REST configuration object.</param>
        /// <returns>The configuration object.</returns>
        public static Rest ConfigureWithUnity(this Rest restConfiguration)
        {
            return RestConfigurator.Configure(null, false);
        }

        /// <summary>
        /// Configures REST Foundation to use a Unity container.
        /// </summary>
        /// <param name="restConfiguration">The REST configuration object.</param>
        /// <param name="registrationBuilder">A delegate to specify additional service dependencies.</param>
        /// <returns>The configuration object.</returns>
        public static Rest ConfigureWithUnity(this Rest restConfiguration, Action<IUnityContainer> registrationBuilder)
        {
            return RestConfigurator.Configure(registrationBuilder, false);
        }
    }
}
