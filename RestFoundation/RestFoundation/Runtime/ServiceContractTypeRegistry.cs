using System;
using System.Collections.Concurrent;

namespace RestFoundation.Runtime
{
    internal static class ServiceContractTypeRegistry
    {
        private static readonly ConcurrentDictionary<string, Type> serviceContractTypes = new ConcurrentDictionary<string, Type>();

        public static Type GetType(string typeAssemblyName)
        {
            if (typeAssemblyName == null) throw new ArgumentNullException("typeAssemblyName");

            return serviceContractTypes.GetOrAdd(typeAssemblyName, t => Type.GetType(typeAssemblyName, true));
        }
    }
}
