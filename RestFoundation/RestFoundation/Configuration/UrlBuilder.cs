// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.Routing;

namespace RestFoundation.Configuration
{
    /// <summary>
    /// Represents a service URL builder.
    /// </summary>
    public sealed class UrlBuilder
    {
        private static readonly Regex serviceNameRegex = new Regex(@"^[_a-zA-Z0-9\-]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

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

            if (!serviceNameRegex.IsMatch(serviceUrl))
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, Resources.Global.InvalidServiceName, serviceUrl));
            }

            return new RouteBuilder(serviceUrl, m_routes);
        }
    }
}
