// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Defines an HTTP authentication type.
    /// </summary>
    public enum AuthenticationType
    {
        /// <summary>
        /// Basic authentication
        /// </summary>
        Basic,

        /// <summary>
        /// Digest authentication
        /// </summary>
        Digest,

        /// <summary>
        /// HMAC authentication
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "HMAC",
                         Justification = "Value is used in the service proxy UI")]
        HMAC
    }
}
