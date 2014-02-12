// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Globalization;
using System.Linq.Expressions;
using RestFoundation.Resources;

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
        /// Ensures a route assert expected to fail does not invoke successfully.
        /// </summary>
        /// <typeparam name="T">The service contract type.</typeparam>
        /// <param name="serviceMethodDelegate">The service method delegate.</param>
        /// <exception cref="RouteAssertException">If the route gets invoked successfully.</exception>
        public void FailsOnInvocation<T>(Expression<Action<T>> serviceMethodDelegate)
        {
            FailsOnInvocation(serviceMethodDelegate, null);
        }

        /// <summary>
        /// Ensures a route assert expected to fail does not invoke successfully. An expected exception message
        /// can be provided for more detailed assert statements.
        /// </summary>
        /// <typeparam name="T">The service contract type.</typeparam>
        /// <param name="serviceMethodDelegate">The service method delegate.</param>
        /// <param name="exceptionMessage">The expected <see cref="RouteAssertException"/> message.</param>
        /// <exception cref="RouteAssertException">
        /// If the route gets invoked successfully; or the expected exception message does not match the actual
        /// exception message.
        /// </exception>
        public void FailsOnInvocation<T>(Expression<Action<T>> serviceMethodDelegate, string exceptionMessage)
        {
            var testRoute = new RouteValidator<T>(m_virtualUrl, m_httpMethod, serviceMethodDelegate);

            try
            {
                testRoute.Validate();
            }
            catch (RouteAssertException ex)
            {
                if (exceptionMessage != null && !String.Equals(exceptionMessage, ex.Message))
                {
                    throw new RouteAssertException(String.Format(CultureInfo.InvariantCulture,
                                                                 Global.FailedRouteWithInvalidExceptionMessage,
                                                                 ex.Message,
                                                                 exceptionMessage));
                }

                return;
            }

            throw new RouteAssertException(Global.FailedRouteInvokedSuccessfully);
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
