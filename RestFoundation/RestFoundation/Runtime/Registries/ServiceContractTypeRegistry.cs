// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using RestFoundation.ServiceProxy;

namespace RestFoundation.Runtime
{
    internal static class ServiceContractTypeRegistry
    {
        private static readonly ConcurrentDictionary<string, Type> serviceContractTypes = new ConcurrentDictionary<string, Type>();
        private static readonly ConcurrentDictionary<Type, IProxyMetadata> serviceContractMetadata = new ConcurrentDictionary<Type, IProxyMetadata>();

        public static Type GetType(string typeAssemblyName)
        {
            if (typeAssemblyName == null)
            {
                throw new ArgumentNullException("typeAssemblyName");
            }

            return serviceContractTypes.GetOrAdd(typeAssemblyName, t => Type.GetType(typeAssemblyName, true));
        }

        public static bool IsServiceContract(Type contractType)
        {
            return serviceContractTypes.Values.Contains(contractType);
        }

        public static ICollection<Type> GetContractTypes()
        {
            return new HashSet<Type>(serviceContractTypes.Values);
        }

        public static IProxyMetadata GetProxyMetadata(Type contractType)
        {
            if (contractType == null)
            {
                throw new ArgumentNullException("contractType");
            }

            IProxyMetadata proxyMetadata = serviceContractMetadata.GetOrAdd(contractType, GenerateProxyMetadata);

            return proxyMetadata is NullProxyMetadata ? null : proxyMetadata;
        }

        internal static IProxyMetadata GenerateProxyMetadata(Type contractType)
        {
            if (contractType.IsClass && contractType.GetInterface(typeof(IProxyMetadata).FullName) != null)
            {
                return InitializeProxyMetadata(contractType);
            }

            var metadataAttribute = Attribute.GetCustomAttribute(contractType, typeof(ProxyMetadataAttribute), false) as ProxyMetadataAttribute;

            if (metadataAttribute == null || metadataAttribute.ProxyMetadataType == null)
            {
                return new NullProxyMetadata();
            }

            return InitializeProxyMetadata(metadataAttribute.ProxyMetadataType);
        }

        private static IProxyMetadata InitializeProxyMetadata(Type proxyMetadataType)
        {
            var proxyMetadata = Rest.Configuration.ServiceLocator.GetService(proxyMetadataType) as IProxyMetadata;

            if (proxyMetadata == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, Resources.Global.InvalidProxyMetadataType, proxyMetadataType.Name));
            }

            proxyMetadata.Initialize();

            return proxyMetadata;
        }
    }
}
