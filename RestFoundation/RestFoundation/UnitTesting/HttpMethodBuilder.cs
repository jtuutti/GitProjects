// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;

namespace RestFoundation.UnitTesting
{
    /// <summary>
    /// Represents an HTTP method builder for route asserts.
    /// </summary>
    public sealed class HttpMethodBuilder
    {
        private readonly string m_relativeUrl;

        internal HttpMethodBuilder(string relativeUrl)
        {
            m_relativeUrl = relativeUrl;
        }

        /// <summary>
        /// Specifies an HTTP method for a route assert.
        /// </summary>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <returns>The route validator builder.</returns>
        public RouteValidatorBuilder WithHttpMethod(HttpMethod httpMethod)
        {
            if (!Enum.IsDefined(typeof(HttpMethod), httpMethod))
            {
                throw new ArgumentOutOfRangeException("httpMethod");
            }

            return new RouteValidatorBuilder(m_relativeUrl, httpMethod);
        }
    }
}
