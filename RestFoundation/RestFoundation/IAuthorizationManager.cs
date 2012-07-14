using System.Collections.Generic;

namespace RestFoundation
{
    /// <summary>
    /// Defines a user authorization manager for security models.
    /// </summary>
    public interface IAuthorizationManager
    {
        /// <summary>
        /// Gets a password or a password hash for the provided user name.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns>The user's password or hash.</returns>
        string GetPassword(string userName);

        /// <summary>
        /// Gets a sequence of security roles for the provided user name.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns>The sequence of user associated roles.</returns>
        IEnumerable<string> GetRoles(string userName);
    }
}
