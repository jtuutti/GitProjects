// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;

namespace RestFoundation.TypeBinders
{
    /// <summary>
    /// Represents a type binder for a specific service method parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public abstract class TypeBinderAttribute : Attribute, ITypeBinder
    {
        /// <summary>
        /// Binds data from an HTTP route, query string or message to a service method parameter.
        /// </summary>
        /// <param name="name">The service method parameter name.</param>
        /// <param name="objectType">The binded object type.</param>
        /// <param name="context">The service context.</param>
        /// <returns>The object instance with the data or null.</returns>
        public abstract object Bind(string name, Type objectType, IServiceContext context);
    }
}
