using System.Collections.Generic;

namespace RestFoundation.Security
{
    /// <summary>
    /// The base authorization manager that does not perform any user authentication and denies any access.
    /// This is a dummy authorization manager implementation designed to be extended.
    /// </summary>
    public class AuthorizationManager : IAuthorizationManager
    {
        /// <summary>
        /// Gets a password or a password hash for the provided user name.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns>The user's password or hash.</returns>
        public virtual string GetPassword(string userName)
        {
            return null;
        }

        /// <summary>
        /// Gets a sequence of security roles for the provided user name.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns>The sequence of user associated roles.</returns>
        public virtual IEnumerable<string> GetRoles(string userName)
        {
            return new string[0];
        }
    }
}
