// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
namespace RestFoundation
{
    /// <summary>
    /// Defines a content negotiator that determines an accepted media type from
    /// the current HTTP request headers or query string data.
    /// </summary>
    public interface IContentNegotiator
    {
        /// <summary>
        /// Gets the preferred accepted media type from the provided HTTP request.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns>A <see cref="string"/> containing the preferred media type.</returns>
        string GetPreferredMediaType(IHttpRequest request);

        /// <summary>
        /// Returns a <see cref="bool"/> value indicating whether the HTTP request
        /// came from a web browser directly.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns>
        /// true if the HTTP request came from a web browser; otherwise, false.
        /// </returns>
        bool IsBrowserRequest(IHttpRequest request);
    }
}
