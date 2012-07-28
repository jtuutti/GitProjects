using System;
using System.Web;
using System.Web.Routing;

namespace RestFoundation.Runtime
{
    internal sealed class BrowserConstraint : IRouteConstraint
    {
        private readonly IContentNegotiator m_contentNegotiator;

        public BrowserConstraint(IContentNegotiator contentNegotiator)
        {
            if (contentNegotiator == null)
            {
                throw new ArgumentNullException("contentNegotiator");
            }

            m_contentNegotiator = contentNegotiator;
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            if (routeDirection != RouteDirection.IncomingRequest)
            {
                return true;
            }

            return m_contentNegotiator.IsBrowserRequest(httpContext.Request);
        }
    }
}
