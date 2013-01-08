// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Specialized;

namespace RestFoundation.Security
{
    /// <summary>
    /// Represents an authorization header.
    /// </summary>
    public class AuthorizationHeader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationHeader"/> class.
        /// </summary>
        /// <param name="authenticationType">The authentication type.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="parameters">A collection of additional parameters.</param>
        public AuthorizationHeader(string authenticationType, string userName, NameValueCollection parameters)
        {
            if (String.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            AuthenticationType = authenticationType;
            UserName = userName;
            Parameters = parameters;
        }

        /// <summary>
        /// Gets the authentication type.
        /// </summary>
        public string AuthenticationType { get; protected set; }

        /// <summary>
        /// Gets the user name.
        /// </summary>
        public string UserName { get; protected set; }

        /// <summary>
        /// Gets the password, if applicable.
        /// </summary>
        public string Password { get; protected internal set; }

        /// <summary>
        /// Gets the parameter collection.
        /// </summary>
        public NameValueCollection Parameters { get; protected set; }
    }
}
