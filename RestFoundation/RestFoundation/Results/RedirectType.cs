// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
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
        /// The resource was moved permanently using the GET HTTP method only.
        /// HTTP code: 301.
        /// </summary>
        MovedPermanently = 301,

        /// <summary>
        /// The resource was found but another URL should be used for the response using
        /// the GET HTTP method only. This is the standard redirect used by the browsers
        /// even though it contradicts the HTTP standard. HTTP code: 302.
        /// </summary>
        Found = 302,

        /// <summary>
        /// The response to the request can be found at another URL using the GET HTTP
        /// method only. HTTP code: 303.
        /// </summary>
        SeeOther = 303,

        /// <summary>
        /// The request should be repeated with another URL; however, future requests should
        /// still use the original URL. Future requests can still use the original URL.
        /// Any HTTP method can be used. HTTP code: 307.
        /// </summary>
        TemporaryRedirect = 307,

        /// <summary>
        /// The request, and all future requests should be repeated using another URL. Any
        /// HTTP method can be used. Warning: this HTTP status code is still experimental and
        /// is not supported by many clients. HTTP code: 308.
        /// </summary>
        PermanentRedirect = 308
    }
}
