using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Web;
using RestFoundation.Collections;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents the default parameter value provider that tries to bind a value
    /// using an associated object type binder. If no binder is associated with the
    /// parameter type, a route value is used. If a parameter is named "resource" or
    /// is decorated with the <see cref="ResourceParameterAttribute"/>, a content
    /// type formatter is used to bind the data.
    /// </summary>
    public class ParameterValueProvider : IParameterValueProvider
    {
        protected const string ResourceParameterName = "resource";

        /// <summary>
        /// Creates a parameter value based on the routing and HTTP data.
        /// </summary>
        /// <param name="parameter">The service method parameters.</param>
        /// <param name="context">The service context.</param>
        /// <param name="isResource">
        /// true if the parameter represents a REST resource; false otherwise. Only 1 resource per
        /// service method is allowed.
        /// </param>
        /// <returns>The created parameter value.</returns>
        public virtual object CreateValue(ParameterInfo parameter, IServiceContext context, out bool isResource)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (parameter == null) throw new ArgumentNullException("parameter");

            IObjectTypeBinder objectTypeBinder = ObjectTypeBinderRegistry.GetBinder(parameter.ParameterType);

            if (objectTypeBinder != null)
            {
                isResource = false;
                return objectTypeBinder.Bind(parameter.ParameterType, parameter.Name, context);
            }

            object routeValue = CreateRouteValue(parameter, context.Request.RouteValues);

            if (routeValue != null)
            {
                isResource = false;
                return routeValue;
            }

            if ((context.Request.Method == HttpMethod.Post || context.Request.Method == HttpMethod.Put || context.Request.Method == HttpMethod.Patch) &&
                String.Equals(ResourceParameterName, parameter.Name, StringComparison.OrdinalIgnoreCase) ||
                Attribute.GetCustomAttribute(parameter, typeof(ResourceParameterAttribute), false) != null ||
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
