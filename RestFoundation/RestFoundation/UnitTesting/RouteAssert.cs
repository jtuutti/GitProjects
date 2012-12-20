// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;

namespace RestFoundation.UnitTesting
{
    /// <summary>
    /// Represents a class to test routes.
    /// </summary>
    public static class RouteAssert
    {
        /// <summary>
        /// Specifies the route relative URL to test.
        /// </summary>
        /// <param name="virtualUrl">The virtual service URL.</param>
        /// <returns>The HTTP method builder.</returns>
        public static HttpMethodBuilder Url(string virtualUrl)
        {
            if (String.IsNullOrEmpty(virtualUrl))
            {
                throw new ArgumentNullException("virtualUrl");
            }

            if (!virtualUrl.TrimStart().StartsWith("~", StringComparison.Ordinal))
            {
                throw new ArgumentException(RestResources.InvalidVirtualUrl, "virtualUrl");
            }

            return new HttpMethodBuilder(virtualUrl);
        }
    }
}
