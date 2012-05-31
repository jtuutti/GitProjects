using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace MvcAlt.Infrastructure
{
    public class HttpVerbConstraint : IRouteConstraint
    {
        private const string HttpMethodOverrideHeader = "X-HTTP-Method-Override";

        public HttpVerbConstraint(params HttpVerb[] allowedMethods)
        {
            if (allowedMethods == null)
            {
                throw new ArgumentNullException("allowedMethods");
            }

            AllowedMethods = allowedMethods.Select(v => v.ToString().ToUpperInvariant()).ToArray();
        }

        public IEnumerable<string> AllowedMethods { get; private set; }

        protected virtual bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            if (route == null)
            {
                throw new ArgumentNullException("route");
            }

            if (String.IsNullOrEmpty(parameterName))
            {
                throw new ArgumentNullException("parameterName");
            }

            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            string method = httpContext.Request.Headers[HttpMethodOverrideHeader];

            if (String.IsNullOrEmpty(method))
            {
                method = httpContext.Request.QueryString[HttpMethodOverrideHeader];
            }

            if (String.IsNullOrEmpty(method))
            {
                method = httpContext.Request.Form[HttpMethodOverrideHeader];
            }

            if (String.IsNullOrEmpty(method))
            {
                method = httpContext.Request.HttpMethod;
            }

            switch (routeDirection)
            {
                case RouteDirection.IncomingRequest:
                    return AllowedMethods.Any(v => v.Equals(method, StringComparison.OrdinalIgnoreCase));
                case RouteDirection.UrlGeneration:
                    string verb = "GET";

                    if (values.ContainsKey(parameterName))
                    {
                        verb = values[parameterName].ToString();
                    }

                    return AllowedMethods.Any(v => v.Equals(verb, StringComparison.OrdinalIgnoreCase));
            }

            return true;
        }

        bool IRouteConstraint.Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return Match(httpContext, route, parameterName, values, routeDirection);
        }
    }
}