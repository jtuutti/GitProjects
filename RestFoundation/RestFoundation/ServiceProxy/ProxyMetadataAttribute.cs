using System;
using System.ComponentModel;

namespace RestFoundation.ServiceProxy
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class ProxyMetadataAttribute : Attribute
    {
        public ProxyMetadataAttribute(Type proxyMetadataType)
        {
            if (proxyMetadataType == null)
            {
                throw new ArgumentNullException("proxyMetadataType");
            }

            ProxyMetadataType = proxyMetadataType;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Type ProxyMetadataType { get; private set; }
    }
}
