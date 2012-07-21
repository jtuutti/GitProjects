using System;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents a service proxy decorator attribute that defines an authentication type and default
    /// credentials associated with a service or a specific service method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
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
        /// Gets or sets an optional  default user name for the service proxy UI.
        /// </summary>
        public string DefaultUserName { get; set; }

        /// <summary>
        /// Gets or sets an optional service relative URL specific for the authentication.
        /// </summary>
        public string ServiceRelativeUrl { get; set; }
    }
}
