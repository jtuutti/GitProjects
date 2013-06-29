// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
namespace RestFoundation
{
    /// <summary>
    /// Defines how a <see cref="IUrlRewriter"/> rewrites request URLs.
    /// </summary>
    public enum UrlRewriteType
    {
        /// <summary>
        /// Rewrites the original URL on the context level.
        /// </summary>
        Rewrite,

        /// <summary>
        /// Redirects the request to the rewritten URL using the
        /// HTTP status code 307 (Temporary Redirect).
        /// </summary>
        TemporaryRedirect,

        /// <summary>
        /// Redirects the request to the rewritten URL using the
        /// HTTP status code 308 (Permanent Redirect). Warning: this
        /// HTTP status code is still experimental and is not supported
        /// by many clients.
        /// </summary>
        PermanentRedirect
    }
}
