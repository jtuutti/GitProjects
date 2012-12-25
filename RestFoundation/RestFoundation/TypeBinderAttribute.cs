using System;

namespace RestFoundation
{
    /// <summary>
    /// Represents a type binder for a specific service method parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public abstract class TypeBinderAttribute : Attribute, ITypeBinder
    {
        /// <summary>
        /// Gets a value indicating whether the binded object is a service method resource.
        /// The default value is false.
        /// </summary>
        public virtual bool IsResource
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Binds data from an HTTP route, query string or message to a service method parameter.
        /// </summary>
        /// <param name="objectType">The binded object type.</param>
        /// <param name="name">The service method parameter name.</param>
        /// <param name="context">The service context.</param>
        /// <returns>The object instance with the data or null.</returns>
        public abstract object Bind(Type objectType, string name, IServiceContext context);
    }
}
