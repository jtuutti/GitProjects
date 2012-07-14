using System.Web;

namespace RestFoundation
{
    /// <summary>
    /// Defines a web browser detector for the service proxy web interface.
    /// </summary>
    public interface IBrowserDetector
    {
        /// <summary>
        /// Returns a <see cref="bool"/> value indicating whether the HTTP request
        /// came from a web browser directly.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns>
        /// true if the HTTP request came from a web browser, false otherwise.
        /// </returns>
        bool IsBrowserRequest(HttpRequestBase request);
    }
}
