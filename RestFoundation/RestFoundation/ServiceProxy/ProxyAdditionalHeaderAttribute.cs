// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents a service proxy decorator attribute that specifies an additional HTTP header
    /// associated with a service contract or a specific service method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class ProxyAdditionalHeaderAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyAdditionalHeaderAttribute"/> class.
        /// </summary>
        /// <param name="name">The header name.</param>
        /// <param name="value">The header value.</param>
        public ProxyAdditionalHeaderAttribute(string name, string value)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets the header name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the header value.
        /// </summary>
        public string Value { get; private set; }
    }
}
