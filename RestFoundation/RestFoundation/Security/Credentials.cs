// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;

namespace RestFoundation.Security
{
    /// <summary>
    /// Represents the credentials of an authenticated and authorized user.
    /// </summary>
    public sealed class Credentials
    {
        private readonly IEnumerable<string> m_roles;

        /// <summary>
        /// Initializes a new instance of the <see cref="Credentials"/> class.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The user password.</param>
        /// <param name="roles">A list of the user roles.</param>
        public Credentials(string userName, string password, IEnumerable<string> roles)
        {
            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }

            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            if (roles == null)
            {
                throw new ArgumentNullException("roles");
            }

            UserName = userName;
            Password = password;
            m_roles = roles;
        }

        /// <summary>
        /// Gets the user name.
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Gets the user password.
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Returns a read only list of user roles.
        /// </summary>
        public string[] GetRoles()
        {
            return new List<string>(m_roles).ToArray();
        }

        /// <summary>
        /// Determines whether the specified <see cref="Credentials"/> is equal to the current <see cref="Credentials"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="Credentials"/> is equal to the current <see cref="Credentials"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="Credentials"/> to compare with the current <see cref="Credentials"/>.</param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var credentialsObj = obj as Credentials;

            return !ReferenceEquals(null, credentialsObj) && Equals(credentialsObj.UserName, UserName);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return UserName.GetHashCode();
        }
    }
}
