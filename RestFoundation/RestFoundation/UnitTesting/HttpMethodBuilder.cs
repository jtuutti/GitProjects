// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;

namespace RestFoundation.UnitTesting
{
    /// <summary>
    /// Represents an HTTP method builder for route asserts.
    /// </summary>
    public sealed class HttpMethodBuilder
    {
        private readonly string m_virtualUrl;

        internal HttpMethodBuilder(string virtualUrl)
        {
            m_virtualUrl = virtualUrl;
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

            return new RouteValidatorBuilder(m_virtualUrl, httpMethod);
        }
    }
}
