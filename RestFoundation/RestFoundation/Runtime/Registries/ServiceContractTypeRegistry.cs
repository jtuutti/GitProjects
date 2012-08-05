// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Concurrent;

namespace RestFoundation.Runtime
{
    internal static class ServiceContractTypeRegistry
    {
        private static readonly ConcurrentDictionary<string, Type> serviceContractTypes = new ConcurrentDictionary<string, Type>();

        public static Type GetType(string typeAssemblyName)
        {
            if (typeAssemblyName == null)
            {
                throw new ArgumentNullException("typeAssemblyName");
            }

            return serviceContractTypes.GetOrAdd(typeAssemblyName, t => Type.GetType(typeAssemblyName, true));
        }

        public static bool IsServiceContract(Type type)
        {
            if (type == null || !type.IsInterface)
            {
                return false;
            }

            return serviceContractTypes.Values.Contains(type);
        }

        public static bool IsServiceImplementation(Type type)
        {
            if (type == null || type.IsInterface || type.IsAbstract)
            {
                return false;
            }

            var contractTypes = serviceContractTypes.Values;

            foreach (Type contractType in contractTypes)
            {
                if (contractType.IsAssignableFrom(type))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
