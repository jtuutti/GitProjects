using System;

namespace RestFoundation
{
    /// <summary>
    /// Defines an object instance activator from the user specified IoC container.
    /// </summary>
    public interface IObjectActivator
    {
        /// <summary>
        /// Creates a new object instance of the specified object type.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <returns>The created object instance.</returns>
        object Create(Type objectType);

        /// <summary>
        /// Injects dependencies into the public properties of the provided object.
        /// </summary>
        /// <param name="obj">The object instance to build up.</param>
        void BuildUp(object obj);
    }
}
