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
        /// <param name="relativeUrl">The relative URL.</param>
        /// <returns>The HTTP method builder.</returns>
        public static HttpMethodBuilder Url(string relativeUrl)
        {
            if (String.IsNullOrEmpty(relativeUrl))
            {
                throw new ArgumentNullException("relativeUrl");
            }

            if (!relativeUrl.TrimStart().StartsWith("~", StringComparison.Ordinal))
            {
                throw new ArgumentException("Relative URL must start with the tilde sign: ~", "relativeUrl");
            }

            return new HttpMethodBuilder(relativeUrl);
        }
    }
}
