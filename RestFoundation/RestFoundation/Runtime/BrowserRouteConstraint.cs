// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Web;
using System.Web.Routing;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents a browser route constraint.
    /// </summary>
    public sealed class BrowserRouteConstraint : IRouteConstraint
    {
        private readonly IContentNegotiator m_contentNegotiator;
        private readonly IHttpRequest m_request;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserRouteConstraint"/> class.
        /// </summary>
        /// <param name="contentNegotiator">The content negotiator.</param>
        /// <param name="request">The HTTP request.</param>
        public BrowserRouteConstraint(IContentNegotiator contentNegotiator, IHttpRequest request)
        {
            if (contentNegotiator == null)
            {
                throw new ArgumentNullException("contentNegotiator");
            }

            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            m_contentNegotiator = contentNegotiator;
            m_request = request;
        }

        /// <summary>
        /// Determines whether the URL parameter contains a valid value for this constraint.
        /// </summary>
        /// <returns>
        /// true if the URL parameter contains a valid value; otherwise, false.
        /// </returns>
        /// <param name="httpContext">An object that encapsulates information about the HTTP request.</param>
        /// <param name="route">The object that this constraint belongs to.</param>
        /// <param name="parameterName">The name of the parameter that is being checked.</param>
        /// <param name="values">An object that contains the parameters for the URL.</param>
        /// <param name="routeDirection">
        /// An object that indicates whether the constraint check is being performed when an incoming request is
        /// being handled or when a URL is being generated.
        /// </param>
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            return routeDirection != RouteDirection.IncomingRequest || m_contentNegotiator.IsBrowserRequest(m_request);
        }
    }
}
