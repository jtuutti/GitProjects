﻿// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Web.Routing;

namespace RestFoundation.Configuration
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
        /// <param name="serviceUrl">The service URL.</param>
        /// <returns>The URL builder.</returns>
        public RouteBuilder MapUrl(string serviceUrl)
        {
            if (serviceUrl == null)
            {
                throw new ArgumentNullException("serviceUrl");
            }

            return new RouteBuilder(serviceUrl, m_routes);
        }
    }
}
