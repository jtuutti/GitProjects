// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.ComponentModel;
using System.Globalization;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Specifies a proxy metadata type for a service contract.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class ProxyMetadataAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyMetadataAttribute"/> class.
        /// </summary>
        /// <param name="proxyMetadataType">A proxy metadata type.</param>
        public ProxyMetadataAttribute(Type proxyMetadataType)
        {
            if (proxyMetadataType == null)
            {
                throw new ArgumentNullException("proxyMetadataType");
            }

            if (proxyMetadataType.GetInterface(typeof(IProxyMetadata).FullName) == null)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, RestResources.InvalidProxyMetadataType, proxyMetadataType.Name), "proxyMetadataType");
            }

            ProxyMetadataType = proxyMetadataType;
        }

        /// <summary>
        /// Gets the proxy metadata type.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Type ProxyMetadataType { get; private set; }
    }
}
