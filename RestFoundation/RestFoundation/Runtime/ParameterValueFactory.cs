using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Web;
using RestFoundation.Collections;

namespace RestFoundation.Runtime
{
    public class ParameterValueFactory : IParameterValueFactory
    {
        protected const string ResourceParameterName = "resource";

        public virtual object CreateValue(IServiceContext context, ParameterInfo parameter, out bool isResource)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (parameter == null) throw new ArgumentNullException("parameter");

            IObjectTypeBinder objectTypeBinder = ObjectTypeBinderRegistry.GetBinder(parameter.ParameterType);

            if (objectTypeBinder != null)
            {
                isResource = false;
                return objectTypeBinder.Bind(context, parameter.ParameterType, parameter.Name);
            }

            object routeValue = CreateRouteValue(parameter, context.Request.RouteValues);

            if (routeValue != null)
            {
                isResource = false;
                return routeValue;
            }

            if ((context.Request.Method == HttpMethod.Post || context.Request.Method == HttpMethod.Put || context.Request.Method == HttpMethod.Patch) &&
                String.Equals(ResourceParameterName, parameter.Name, StringComparison.OrdinalIgnoreCase) ||
                Attribute.GetCustomAttribute(parameter, typeof(BindResourceAttribute), false) != null ||
                parameter.ParameterType == typeof(IEnumerable<IUploadedFile>) || parameter.ParameterType == typeof(ICollection<IUploadedFile>))
            {
                isResource = true;
                return CreateResourceValue(parameter, context);
            }

            isResource = false;
            return null;
        }

        protected virtual object CreateRouteValue(ParameterInfo parameter, IObjectValueCollection routeValues)
        {
            if (parameter == null) throw new ArgumentNullException("parameter");
            if (routeValues == null) throw new ArgumentNullException("routeValues");

            object routeValue = routeValues.TryGet(parameter.Name);

            if (routeValue == null)
            {
                return null;
            }

            object value;

            if (!SafeConvert.TryChangeType(routeValue, parameter.ParameterType, out value))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, "Not Found");
            }

            return value;
        }

        protected virtual object CreateResourceValue(ParameterInfo parameter, IServiceContext context)
        {
            if (parameter == null) throw new ArgumentNullException("parameter");
            if (context == null) throw new ArgumentNullException("context");

            IContentTypeFormatter formatter = ContentTypeFormatterRegistry.GetFormatter(context.Request.Headers.ContentType);

            if (formatter == null)
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType, "Unsupported content type provided");
            }

            object argumentValue;

            try
            {
                argumentValue = formatter.FormatRequest(context, parameter.ParameterType);
            }
            catch (Exception ex)
            {
                if (ExceptionUnwrapper.IsDirectResponseException(ex))
                {
                    throw;
                }

                var httpException = ex as HttpException;

                if (httpException != null)
                {
                    throw new HttpResponseException((HttpStatusCode) httpException.GetHttpCode(), httpException.Message);
                }

                throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid resource body provided");
            }

            return argumentValue;
        }
    }
}
