// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System.Collections.Specialized;

namespace RestFoundation
{
    /// <summary>
    /// Defines a URL rewriting interface.
    /// </summary>
    public interface IUrlRewriter
    {
        /// <summary>
        /// Gets a value indicating how the requests URLs will be rewritten.
        /// </summary>
        UrlRewriteType RewriteType { get; }

        /// <summary>
        /// Returns a rewritten URL for the current HTTP request.
        /// </summary>
        /// <param name="relativeUrl">The current request relative url.</param>
        /// <param name="requestHeaders">The current request headers.</param>
        /// <returns>
        /// A <see cref="string"/> containing the rewritten URL, or null if the
        /// URL does not need to be rewritten.
        /// </returns>
        string RewriteUrl(string relativeUrl, NameValueCollection requestHeaders);
    }
}
