// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Linq.Expressions;

namespace RestFoundation.UnitTesting
{
    /// <summary>
    /// Represents a route validator builder.
    /// </summary>
    public sealed class RouteValidatorBuilder
    {
        private readonly HttpMethod m_httpMethod;
        private readonly string m_virtualUrl;

        internal RouteValidatorBuilder(string virtualUrl, HttpMethod httpMethod)
        {
            m_virtualUrl = virtualUrl;
            m_httpMethod = httpMethod;
        }

        /// <summary>
        /// Specifies the service method delegate that should be invoked by the asserted route.
        /// </summary>
        /// <typeparam name="T">The service contract type.</typeparam>
        /// <param name="serviceMethodDelegate">The service method delegate.</param>
        /// <exception cref="RouteAssertException">If the route does not match the invoked delegate.</exception>
        public void Invokes<T>(Expression<Action<T>> serviceMethodDelegate)
        {
            var testRoute = new RouteValidator<T>(m_virtualUrl, m_httpMethod, serviceMethodDelegate);
            testRoute.Validate();
        }
    }
}
