using System;
using System.Web;
using System.Web.Routing;

namespace RestFoundation.Runtime
{
    internal sealed class BrowserConstraint : IRouteConstraint
    {
        private readonly IBrowserDetector m_browserDetector;

        public BrowserConstraint(IBrowserDetector browserDetector)
        {
            if (browserDetector == null) throw new ArgumentNullException("browserDetector");

            m_browserDetector = browserDetector;
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (httpContext == null) throw new ArgumentNullException("httpContext");

            if (routeDirection != RouteDirection.IncomingRequest)
            {
                return true;
            }

            return m_browserDetector.IsBrowserRequest(httpContext.Request);
        }
    }
}
