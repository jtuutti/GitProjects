// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using RestFoundation.Context;

namespace RestFoundation.Runtime
{
    internal sealed class HttpHandlerRouteConstraint : IRouteConstraint
    {
        private readonly IReadOnlyCollection<HttpMethod> m_allowedMethods;

        public HttpHandlerRouteConstraint(IReadOnlyCollection<HttpMethod> allowedMethods)
        {
            if (allowedMethods == null)
            {
                throw new ArgumentNullException("allowedMethods");
            }

            m_allowedMethods = allowedMethods;
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            if (m_allowedMethods.Count == 0)
            {
                return true;
            }

            HttpMethod method = httpContext.GetOverriddenHttpMethod();

            return m_allowedMethods.Contains(method);
        }
    }
}
