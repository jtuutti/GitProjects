using System;
using System.Web.Routing;

namespace RestFoundation
{
    /// <summary>
    /// Represents a service URL builder.
    /// </summary>
    public sealed class UrlBuilder
    {
        private readonly RouteCollection m_routes;
        private readonly IHttpMethodResolver m_httpMethodResolver;
        private readonly IContentNegotiator m_contentNegotiator;

        internal UrlBuilder(RouteCollection routes, IHttpMethodResolver httpMethodResolver, IContentNegotiator contentNegotiator)
        {
            if (routes == null) throw new ArgumentNullException("routes");
            if (httpMethodResolver == null) throw new ArgumentNullException("httpMethodResolver");
            if (contentNegotiator == null) throw new ArgumentNullException("contentNegotiator");

            m_routes = routes;
            m_httpMethodResolver = httpMethodResolver;
            m_contentNegotiator = contentNegotiator;
        }

        /// <summary>
        /// Maps the provided relative URL to a service or a web forms page.
        /// </summary>
        /// <param name="url">The relative URL.</param>
        /// <returns>The URL builder.</returns>
        public RouteBuilder MapUrl(string url)
        {
            if (String.IsNullOrEmpty(url)) throw new ArgumentNullException("url");

            return new RouteBuilder(url, m_routes, m_httpMethodResolver, m_contentNegotiator, false);
        }
    }
}
