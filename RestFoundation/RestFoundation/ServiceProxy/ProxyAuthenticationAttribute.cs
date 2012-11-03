// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents a service proxy decorator attribute that defines an authentication type and default
    /// credentials associated with a service or a specific service method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ProxyAuthenticationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyAuthenticationAttribute"/> class.
        /// </summary>
        /// <param name="authentication">The authentication type.</param>
        public ProxyAuthenticationAttribute(AuthenticationType authentication)
        {
            Authentication = authentication;
        }

        /// <summary>
        /// Gets the authentication type.
        /// </summary>
        public AuthenticationType Authentication { get; private set; }

        /// <summary>
        /// Gets or sets an optional default user name for the service proxy UI.
        /// </summary>
        public string DefaultUserName { get; set; }

        /// <summary>
        /// Gets or sets an optional service relative URL for the authentication to match.
        /// </summary>
        public string RelativeUrlToMatch { get; set; }
    }
}
