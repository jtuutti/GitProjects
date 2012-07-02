using System;
using System.Web.Routing;
using RestFoundation.Runtime;
using RestFoundation.Test.HttpContext;

namespace RestFoundation.Test
{
    public static class RequestContextMockFactory
    {
        public static RequestContext Create(Type serviceContractType, string relativeUrl, string urlTemplate, string httpMethod)
        {
            if (serviceContractType == null) throw new ArgumentNullException("serviceContractType");
            if (String.IsNullOrEmpty(relativeUrl)) throw new ArgumentNullException("relativeUrl");
            if (String.IsNullOrEmpty(urlTemplate)) throw new ArgumentNullException("urlTemplate");
            if (String.IsNullOrEmpty(httpMethod)) throw new ArgumentNullException("httpMethod");
            if (!serviceContractType.IsInterface) throw new ArgumentException("Service contract type must be an interface", "serviceContractType");

            var httpContext = new TestHttpContext(relativeUrl, httpMethod);

            var routeData = new RouteData();
            routeData.Values.Add(RouteConstants.ServiceContractType, serviceContractType.AssemblyQualifiedName);
            routeData.Values.Add(RouteConstants.ServiceUrl, relativeUrl);
            routeData.Values.Add(RouteConstants.UrlTemplate, urlTemplate);

            return new RequestContext(httpContext, routeData);
        }
    }
}
