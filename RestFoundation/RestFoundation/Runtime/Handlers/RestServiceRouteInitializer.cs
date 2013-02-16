// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Net;
using System.Web.Routing;

namespace RestFoundation.Runtime.Handlers
{
    internal sealed class RestServiceRouteInitializer
    {
        private readonly IRestServiceHandler m_handler;

        public RestServiceRouteInitializer(IRestServiceHandler handler)
        {
            m_handler = handler;
        }

        public RestServiceRouteInfo Initialize(RequestContext requestContext)
        {
            if (requestContext.RouteData == null || requestContext.RouteData.Values == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, RestResources.MissingRouteData);
            }

            if (!RestHttpModule.IsInitialized)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, RestResources.MissingRestHttpModule);
            }

            if (UnvalidatedHandlerRegistry.IsUnvalidated(m_handler))
            {
                requestContext.HttpContext.Items[ServiceRequestValidator.UnvalidatedHandlerKey] = Boolean.TrueString;
            }

            SetRouteRequestUniqueId(requestContext);

            return GenerateRouteInfo(requestContext);
        }

        private static void SetRouteRequestUniqueId(RequestContext requestContext)
        {
            requestContext.HttpContext.Items["ServiceExecutionId"] = Guid.NewGuid();
        }

        private static RestServiceRouteInfo GenerateRouteInfo(RequestContext requestContext)
        {
            var routeInfo = new RestServiceRouteInfo
            {
                ServiceUrl = (string) requestContext.RouteData.Values[RouteConstants.ServiceUrl],
                ServiceContractTypeName = (string) requestContext.RouteData.Values[RouteConstants.ServiceContractType],
                UrlTemplate = (string) requestContext.RouteData.Values[RouteConstants.UrlTemplate]
            };

            if (String.IsNullOrEmpty(routeInfo.ServiceUrl) || String.IsNullOrEmpty(routeInfo.ServiceContractTypeName) || routeInfo.UrlTemplate == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, RestResources.NotFound);
            }
            return routeInfo;
        }
    }
}
