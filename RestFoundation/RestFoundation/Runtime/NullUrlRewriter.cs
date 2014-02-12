// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System.Collections.Specialized;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents the default URL rewriter that does not rewrite URLs.
    /// </summary>
    public sealed class NullUrlRewriter : IUrlRewriter
    {
        /// <summary>
        /// Gets a value indicating how the requests URLs will be rewritten.
        /// </summary>
        public UrlRewriteType RewriteType
        {
            get
            {
                return UrlRewriteType.Rewrite;
            }
        }

        /// <summary>
        /// Returns a rewritten URL for the current HTTP request.
        /// </summary>
        /// <param name="relativeUrl">The current request relative url.</param>
        /// <param name="requestHeaders">The current request headers.</param>
        /// <returns>null</returns>
        public string RewriteUrl(string relativeUrl, NameValueCollection requestHeaders)
        {
            return null;
        }
    }
}
