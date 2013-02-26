// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Globalization;
using System.Net;
using System.Web;
using System.Web.Routing;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents a service proxy helper that generates URLs from name route names.
    /// </summary>
    public static class ProxyUrlHelper
    {
        /// <summary>
        /// Gets the URL represented by the named route.
        /// </summary>
        /// <param name="response">The HTTP context response.</param>
        /// <param name="routeName">The route name.</param>
        /// <returns>The generated URL.</returns>
        public static string GetByRouteName(HttpResponse response, string routeName)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            if (String.IsNullOrEmpty(routeName))
            {
                throw new ArgumentNullException("routeName");
            }

            VirtualPathData pathData = RouteTable.Routes.GetVirtualPath(null, routeName, new RouteValueDictionary());

            if (pathData != null && !String.IsNullOrEmpty(pathData.VirtualPath))
            {
                return pathData.VirtualPath;
            }

            response.Clear();
            response.StatusCode = (int) HttpStatusCode.NotFound;
            response.StatusDescription = String.Format(CultureInfo.InvariantCulture, RestResources.InvalidNamedRoute, routeName);

            return null;
        }
    }
}
