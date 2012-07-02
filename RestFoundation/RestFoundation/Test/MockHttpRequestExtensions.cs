using System;
using System.Web.Routing;
using RestFoundation.Collections.Concrete;

namespace RestFoundation.Test
{
    public static class HttpRequestExtensions
    {
        public static void SetRouteValues(this IHttpRequest request, object values)
        {
            if (values == null) throw new ArgumentNullException("values");

            SetRouteValues(request, new RouteValueDictionary(values));
        }

        public static void SetRouteValues(this IHttpRequest request, RouteValueDictionary values)
        {
            if (values == null) throw new ArgumentNullException("values");

            var mockRequest = request as MockHttpRequest;

            if (mockRequest == null)
            {
                throw new NotSupportedException("This method only supports mocked HTTP request objects");
            }

            mockRequest.RouteValues = new ObjectValueCollection(values);
        }
    }
}
