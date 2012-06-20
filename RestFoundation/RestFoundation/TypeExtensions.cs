using System;
using RestFoundation.Runtime;

namespace RestFoundation
{
    public static class TypeExtensions
    {
        public static bool IsRestBehavior(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return typeof(IServiceBehavior).IsAssignableFrom(type);
        }

        public static bool IsRestContext(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (type.IsInterface)
            {
                return type == typeof(IHttpRequest) || type == typeof(IHttpResponse) ||
                       type == typeof(IServiceContext);
            }

            Type[] interfaces = type.GetInterfaces();

            return (Array.IndexOf(interfaces, typeof(IHttpRequest)) >= 0 ||
                    Array.IndexOf(interfaces, typeof(IHttpResponse)) >= 0 ||
                    Array.IndexOf(interfaces, typeof(IServiceContext)) >= 0);
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
