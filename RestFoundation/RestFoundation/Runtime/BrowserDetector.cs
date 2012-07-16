using System;
using System.Linq;
using System.Web;
using RestFoundation.Collections.Specialized;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents a default browser detector for the service proxy interface.
    /// It inspects the Accept HTTP header for the preferred content type. If
    /// the text/html content type is accepted and has a higher quality factor
    /// than other accepted content types, the HTTP client is considered to be
    /// a browser.
    /// </summary>
    public class BrowserDetector : IBrowserDetector
    {
        private const string HtmlContentType = "text/html";

        /// <summary>
        /// Returns a <see cref="bool"/> value indicating whether the HTTP request
        /// came from a web browser directly.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns>
        /// true if the HTTP request came from a web browser; otherwise, false.
        /// </returns>
        public virtual bool IsBrowserRequest(HttpRequestBase request)
        {
            if (request == null) throw new ArgumentNullException("request");

            if (!"GET".Equals(request.HttpMethod, StringComparison.OrdinalIgnoreCase) &&
                !"HEAD".Equals(request.HttpMethod, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string acceptedValue = request.QueryString["X-Accept-Override"];

            if (String.IsNullOrEmpty(acceptedValue))
            {
                acceptedValue = request.Headers.Get("Accept");
            }

            var acceptTypeCollection = new AcceptValueCollection(acceptedValue);
            var contentTypes = ContentFormatterRegistry.GetContentTypes();

            foreach (string acceptedType in acceptTypeCollection.AcceptedNames)
            {
                if (String.Equals(HtmlContentType, acceptedType, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (contentTypes.Contains(acceptedType, StringComparer.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return acceptTypeCollection.CanAccept(HtmlContentType);
        }
    }
}
