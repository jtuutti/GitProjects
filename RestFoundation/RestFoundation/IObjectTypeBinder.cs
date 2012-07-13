using System;

namespace RestFoundation
{
    /// <summary>
    /// Defines a binder for service method parameters of a specific object type.
    /// The type is specified during the binder registration through a <see cref="Rest"/> instance.
    /// </summary>
    public interface IObjectTypeBinder
    {
        /// <summary>
        /// Binds data from an HTTP route, query string or message to a service method parameter.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="objectType">The binded object type.</param>
        /// <param name="name">The service method parameter name.</param>
        /// <returns>The object instance with the data or null.</returns>
        object Bind(IServiceContext context, Type objectType, string name);
    }
}
