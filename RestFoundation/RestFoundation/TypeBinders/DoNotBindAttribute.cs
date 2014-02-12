// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;

namespace RestFoundation.TypeBinders
{
    /// <summary>
    /// Represents a type binder that prevents the parameter from getting binded.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class DoNotBindAttribute : TypeBinderAttribute
    {
        /// <summary>
        /// Binds data from an HTTP route, query string or message to a service method parameter.
        /// </summary>
        /// <param name="name">The service method parameter name.</param>
        /// <param name="objectType">The binded object type.</param>
        /// <param name="context">The service context.</param>
        /// <returns>The object instance with the data or null.</returns>
        /// <exception cref="NotSupportedException">This method is not supposed to be executed.</exception>
        public override object Bind(string name, Type objectType, IServiceContext context)
        {
            throw new NotSupportedException();
        }
    }
}
