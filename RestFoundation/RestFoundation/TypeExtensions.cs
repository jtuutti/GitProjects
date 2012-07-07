using System;
using System.Collections.Generic;
using RestFoundation.Runtime;

namespace RestFoundation
{
    public static class TypeExtensions
    {
        private static readonly HashSet<Type> dependencyTypes = new HashSet<Type>
        {
            typeof(IServiceContext),
            typeof(IHttpRequest),
            typeof(IHttpResponse),
            typeof(IServiceCache),
            typeof(ILogger)
        };

        public static bool IsRestBehavior(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return typeof(IServiceBehavior).IsAssignableFrom(type);
        }

        public static bool IsRestDependency(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return type.IsInterface && dependencyTypes.Contains(type);
        }

        public static bool IsRestContract(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return ServiceContractTypeRegistry.IsServiceContract(type);
        }

        public static bool IsRestImplementation(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return ServiceContractTypeRegistry.IsServiceImplementation(type);
        }
    }
}
