// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.Unity;
using RestFoundation.Configuration;
using RestFoundation.Unity;

namespace RestFoundation
{
    /// <summary>
    /// Defines extensions for a <see cref="Rest"/> object to configure REST Foundation to use a Unity container.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RestExtensions
    {
        /// <summary>
        /// Initializes REST Foundation configuration with the provided Unity container.
        /// </summary>
        /// <param name="restConfiguration">The REST configuration options object.</param>
        /// <param name="container">The Unity container.</param>
        /// <returns>The configuration object.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "restConfiguration",
                         Justification = "This extension method should be able to execute if the configuration has not been set up yet.")]
        public static RestOptions InitializeWithUnity(this Rest restConfiguration, IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            return RestConfigurator.Configure(container, false);
        }
    }
}
