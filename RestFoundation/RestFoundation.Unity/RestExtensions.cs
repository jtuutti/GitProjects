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
        /// Configures REST Foundation with the provided Unity container.
        /// </summary>
        /// <param name="restConfiguration">The REST configuration object.</param>
        /// <param name="container">The Unity container.</param>
        /// <returns>The configuration object.</returns>
        public static Rest ConfigureWithUnity(this Rest restConfiguration, IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            return RestConfigurator.Configure(container, false);
        }
    }
}
