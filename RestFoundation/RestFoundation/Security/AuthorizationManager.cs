// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
namespace RestFoundation.Security
{
    /// <summary>
    /// The base authorization manager that does not perform any user authentication and denies any access.
    /// This is a dummy authorization manager implementation designed to be extended.
    /// </summary>
    public class AuthorizationManager : IAuthorizationManager
    {
        /// <summary>
        /// Gets an authenticated and authorized user credentials by the provided user name.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns>The user credentials; or null if the username is invalid.</returns>
        public Credentials GetCredentials(string userName)
        {
            return null;
        }
    }
}
