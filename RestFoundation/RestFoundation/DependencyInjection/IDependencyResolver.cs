using System;
using System.Collections.Generic;

namespace RestFoundation.DependencyInjection
{
    /// <summary>
    /// Represents an abstraction over an IoC container that resolves object implementations by their abstraction types.
    /// </summary>
    public interface IDependencyResolver
    {
        /// <summary>
        /// Resolves a singly registered object by its abstraction type.
        /// </summary>
        /// <param name="type">The abstraction type.</param>
        /// <returns>The object instance.</returns>
        object Resolve(Type type);

        /// <summary>
        /// Resolves a singly registered object by its abstraction type.
        /// </summary>
        /// <typeparam name="T">The abstraction type.</typeparam>
        /// <returns>The object instance.</returns>
        T Resolve<T>();

        /// <summary>
        /// Resolves a singly registered object by its abstraction type and the provided registration key.
        /// </summary>
        /// <param name="key">The object registration key.</param>
        /// <param name="type">The abstraction type.</param>
        /// <returns>The object instance.</returns>
        object Resolve(string key, Type type);

        /// <summary>
        /// Resolves a singly registered object by its abstraction type and the provided registration key.
        /// </summary>
        /// <typeparam name="T">The abstraction type.</typeparam>
        /// <param name="key">The object registration key.</param>
        /// <returns>The object instance.</returns>
        T Resolve<T>(string key);

        /// <summary>
        /// Resolves all implementation instances of the specified abstraction type.
        /// </summary>
        /// <param name="type">The abstraction type.</param>
        /// <returns>A sequence of implementation instances.</returns>
        IEnumerable<object> ResolveAll(Type type);

        /// <summary>
        /// Resolves all implementation instances of the specified abstraction type.
        /// </summary>
        /// <typeparam name="T">The abstraction type.</typeparam>
        /// <returns>A sequence of implementation instances.</returns>
        IEnumerable<T> ResolveAll<T>();

        /// <summary>
        /// Injects dependencies into the public properties of the provided object.
        /// </summary>
        /// <param name="obj">The object instance to build up.</param>
        void BuildUp(object obj);
    }
}
