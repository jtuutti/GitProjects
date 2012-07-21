using System;
using RestFoundation.StructureMap;
using StructureMap.Configuration.DSL;

namespace RestFoundation
{
    /// <summary>
    /// Defines extensions for a <see cref="Rest"/> object to configure REST Foundation to use a StructureMap container.
    /// </summary>
    public static class RestExtensions
    {
        /// <summary>
        /// Configures REST Foundation to use a StructureMap container.
        /// </summary>
        /// <param name="restConfiguration">The REST configuration object.</param>
        /// <returns>The configuration object.</returns>
        public static Rest ConfigureWithStructureMap(this Rest restConfiguration)
        {
            return RestConfigurator.Configure(null, false);
        }

        /// <summary>
        /// Configures REST Foundation to use a StructureMap container.
        /// </summary>
        /// <param name="restConfiguration">The REST configuration object.</param>
        /// <param name="registrationBuilder">A delegate to specify additional service dependencies.</param>
        /// <returns>The configuration object.</returns>
        public static Rest ConfigureWithStructureMap(this Rest restConfiguration, Action<Registry> registrationBuilder)
        {
            return RestConfigurator.Configure(registrationBuilder, false);
        }
    }
}
