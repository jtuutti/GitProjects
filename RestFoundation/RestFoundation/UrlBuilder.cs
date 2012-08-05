// <copyright>
// Dmitry Starosta, 2012
// </copyright>
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

        internal UrlBuilder(RouteCollection routes)
        {
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }

            m_routes = routes;
        }

        /// <summary>
        /// Maps the provided relative URL to a service or a web forms page.
        /// </summary>
        /// <param name="url">The relative URL.</param>
        /// <returns>The URL builder.</returns>
        public RouteBuilder MapUrl(string url)
        {
            if (String.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            return new RouteBuilder(url, m_routes, null);
        }
    }
}
