// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
namespace RestFoundation.Security
{
    /// <summary>
    /// Defines a user authorization manager for security models.
    /// </summary>
    public interface IAuthorizationManager
    {
        /// <summary>
        /// Gets an authenticated and authorized user credentials by the provided user name.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns>The user credentials; or null if the user name is invalid.</returns>
        Credentials GetCredentials(string userName);
    }
}
