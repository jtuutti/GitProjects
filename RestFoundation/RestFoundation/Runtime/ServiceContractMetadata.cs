// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using RestFoundation.ServiceProxy;

namespace RestFoundation.Runtime
{
    internal struct ServiceContractMetadata : IEquatable<ServiceContractMetadata>
    {
        private readonly Type m_type;
        private readonly IProxyMetadata m_proxyMetadata;

        public ServiceContractMetadata(Type type, IProxyMetadata proxyMetadata)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            m_type = type;
            m_proxyMetadata = proxyMetadata;
        }

        public Type Type
        {
            get
            {
                return m_type;
            }
        }

        public IProxyMetadata ProxyMetadata
        {
            get
            {
                return m_proxyMetadata;
            }
        }

        public bool Equals(ServiceContractMetadata other)
        {
            return m_type == other.m_type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is ServiceContractMetadata && Equals((ServiceContractMetadata) obj);
        }

        public override int GetHashCode()
        {
            return m_type.GetHashCode();
        }
    }
}
