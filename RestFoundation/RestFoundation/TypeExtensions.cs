using System;
using System.Collections.Generic;
using RestFoundation.Runtime;

namespace RestFoundation
{
    /// <summary>
    /// Defines type extensions designed to simpify IoC registrations.
    /// </summary>
    public static class TypeExtensions
    {
        private static readonly HashSet<Type> dependencyTypes = new HashSet<Type>
        {
            typeof(IServiceContext),
            typeof(IHttpRequest),
            typeof(IHttpResponse),
            typeof(IServiceCache)
        };

        /// <summary>
        /// Returns a value indicating whether the type represents a REST Foundation behavior.
        /// </summary>
        /// <param name="type">The object type.</param>
        /// <returns>
        /// true if the type contains REST Foundation behavior; otherwise, false.
        /// </returns>
        public static bool IsRestBehavior(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return typeof(IServiceBehavior).IsAssignableFrom(type);
        }

        /// <summary>
        /// Returns a value indicating whether the type represents a REST Foundation dependency.
        /// Dependencies include service context, HTTP request and response and cache objects.
        /// </summary>
        /// <param name="type">The object type.</param>
        /// <returns>
        /// true if the type is REST Foundation dependency; otherwise, false.
        /// </returns>
        public static bool IsRestDependency(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return type.IsInterface && dependencyTypes.Contains(type);
        }

        /// <summary>
        /// Returns a value indicating whether the type is a REST Foundation service contract interface.
        /// </summary>
        /// <param name="type">The object type.</param>
        /// <returns>
        /// true if the type is a service contract; otherwise, false.
        /// </returns>
        public static bool IsRestContract(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return ServiceContractTypeRegistry.IsServiceContract(type);
        }

        /// <summary>
        /// Returns a value indicating whether the type is a REST Foundation service implementation.
        /// </summary>
        /// <param name="type">The object type.</param>
        /// <returns>
        /// true if the type is a service implementation; otherwise, false.
        /// </returns>
        public static bool IsRestService(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return ServiceContractTypeRegistry.IsServiceImplementation(type);
        }
    }
}
