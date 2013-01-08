// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Web;
using System.Web.Routing;

namespace RestFoundation.Runtime
{
    internal sealed class BrowserRouteConstraint : IRouteConstraint
    {
        private readonly IContentNegotiator m_contentNegotiator;
        private readonly IHttpRequest m_request;

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
