// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

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

        public static ICollection<Type> GetContractTypes()
        {
            return new HashSet<Type>(serviceContractTypes.Values);
        }
    }
}
