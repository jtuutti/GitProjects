using System.Diagnostics.CodeAnalysis;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents a redirect type.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue",
                     Justification = "Redirect type cannot be of type 0")]
    public enum RedirectType
    {
        /// <summary>
        /// The resource was moved permanently.
        /// </summary>
        Permanent = 301,

        /// <summary>
        /// The resource was found but a different URL should be used for the response.
        /// This is the standard redirect used by the browsers even though it contradicts
        /// the HTTP standard.
        /// </summary>
        Found = 302,

        /// <summary>
        /// The response to the request can be found at another URL.
        /// </summary>
        SeeOther = 303,

        /// <summary>
        /// The resource was moved temporarily. Future requests can still use the original URL.
        /// </summary>
        Temporary = 307
    }
}
