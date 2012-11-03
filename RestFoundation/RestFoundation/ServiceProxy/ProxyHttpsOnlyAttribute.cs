// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents a service proxy decorator attribute that specifies that the service method
    /// requires a secure HTTPS connection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ProxyHttpsOnlyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyHttpsOnlyAttribute"/> class.
        /// </summary>
        public ProxyHttpsOnlyAttribute()
        {
            Port = 443;
        }

        /// <summary>
        /// Gets or sets the HTTPS port number. The default port is 443.
        /// </summary>
        public int Port { get; set; }
    }
}
