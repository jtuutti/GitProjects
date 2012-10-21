// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.Routing;
using RestFoundation.Runtime.Handlers;
using RestFoundation.Security;
using RestFoundation.ServiceProxy;

namespace RestFoundation
{
    /// <summary>
    /// Represents the service help and proxy interface configuration.
    /// </summary>
    public sealed class ProxyConfiguration
    {
        internal ProxyConfiguration()
        {
        }

        /// <summary>
        /// Enables HTML based interface for the service help pages and HTTP proxy under the relative URL "help".
        /// </summary>
        /// <returns>The configuration object.</returns>
        public ProxyConfiguration Enable()
        {
            return EnableWithRelativeUrl("help");
        }

        /// <summary>
        /// Enables HTML based interface for the service help pages and HTTP proxy.
        /// IMPORTANT: If you are setting the relative URL parameter to something other than "help",
        /// make sure to adjust the location/path setting in the Web.config accordingly to avoid
        /// ASP .NET validation errors.
        /// </summary>
        /// <param name="relativeUrl">The relative URL path for the service help and proxy.</param>
        /// <returns>The configuration object.</returns>
        /// <exception cref="ArgumentException">If the relative URL contains invalid characters.</exception>
        public ProxyConfiguration EnableWithRelativeUrl(string relativeUrl)
        {
            if (relativeUrl == null)
            {
                throw new ArgumentNullException("relativeUrl");
            }

            if (Rest.Active.IsServiceProxyInitialized)
            {
                throw new InvalidOperationException(RestResources.ProxyAlreadyInitialized);
            }

            if (!Regex.IsMatch(relativeUrl, "^[0-9a-zA-Z]+([0-9a-zA-Z-]*[0-9a-zA-Z]+)?$"))
            {
                string message = String.Format(CultureInfo.InvariantCulture,
                                               "{0}. Relative URL '{1}' does not meet those requirements.",
                                               "Service help/proxy relative URL can only contain letters and numbers with optional dashes in between",
                                               relativeUrl);

                throw new ArgumentException(message, "relativeUrl");
            }

            Rest.Active.IsServiceProxyInitialized = true;
            Rest.Active.ServiceProxyRelativeUrl = relativeUrl.ToLowerInvariant();

            ProxyPathProvider.AppInitialize();

            RouteTable.Routes.MapPageRoute("ProxyIndex", relativeUrl + "/index", "~/index.aspx");
            RouteTable.Routes.MapPageRoute(String.Empty, relativeUrl + "/metadata", "~/metadata.aspx");
            RouteTable.Routes.MapPageRoute(String.Empty, relativeUrl + "/output", "~/output.aspx");
            RouteTable.Routes.MapPageRoute(String.Empty, relativeUrl + "/proxy", "~/proxy.aspx");
            RouteTable.Routes.Add(new Route(relativeUrl, new ProxyRootHandler()));

            return this;
        }

        /// <summary>
        /// Sets the JQuery URL if the default CDN location "http://code.jquery.com/jquery-1.7.2.min.js" is not desired.
        /// The service help and proxy interface was designed with JQuery version 1.7.2.
        /// </summary>
        /// <param name="url">The JQuery HTTP/HTTPS URL.</param>
        /// <returns>The configuration object.</returns>
        public ProxyConfiguration SetJQueryUrl(string url)
        {
            if (String.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            SetJQueryUrl(new Uri(url, UriKind.RelativeOrAbsolute));
            return this;
        }

        /// <summary>
        /// Sets the JQuery URL if the default CDN location "http://code.jquery.com/jquery-1.7.2.min.js" is not desired.
        /// The service help and proxy interface was designed with JQuery version 1.7.2.
        /// </summary>
        /// <param name="url">The JQuery HTTP/HTTPS URL.</param>
        /// <returns>The configuration object.</returns>
        public ProxyConfiguration SetJQueryUrl(Uri url)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            Rest.Active.JQueryUrl = url.ToString();
            return this;
        }

        /// <summary>
        /// Requires the service proxy to be accessed through HTTPS only.
        /// </summary>
        /// <returns>The configuration object.</returns>
        public ProxyConfiguration HttpsOnly()
        {
            Rest.Active.ServiceProxyHttpsOnly = true;
            return this;
        }

        /// <summary>
        /// Sets basic authentication for the service proxy and authorizes users through the provided authorization manager
        /// instance.
        /// </summary>
        /// <param name="authorizationManager">The authorization manager.</param>
        /// <returns>The configuration object.</returns>
        public ProxyConfiguration RequireAuthorization(IAuthorizationManager authorizationManager)
        {
            if (authorizationManager == null)
            {
                throw new ArgumentNullException("authorizationManager");
            }

            Rest.Active.ServiceProxyAuthorizationManager = authorizationManager;
            return this;
        }
    }
}
