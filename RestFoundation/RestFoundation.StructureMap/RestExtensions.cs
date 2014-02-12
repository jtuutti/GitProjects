// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using RestFoundation.Configuration;
using RestFoundation.StructureMap;
using IContainer = StructureMap.IContainer;

namespace RestFoundation
{
    /// <summary>
    /// Defines extensions for a <see cref="Rest"/> object to configure REST Foundation to use a StructureMap container.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RestExtensions
    {
        /// <summary>
        /// Initializes REST Foundation configuration with the provided StructureMap container.
        /// </summary>
        /// <param name="restConfiguration">The REST configuration options object.</param>
        /// <param name="container">The StructureMap container.</param>
        /// <returns>The configuration object.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "restConfiguration",
                         Justification = "This extension method should be able to execute if the configuration has not been set up yet.")]
        public static RestOptions InitializeWithStructureMap(this Rest restConfiguration, IContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            return RestConfigurator.Configure(container, false);
        }
    }
}
