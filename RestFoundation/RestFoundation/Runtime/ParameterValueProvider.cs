// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web;
using RestFoundation.Collections;
using RestFoundation.Formatters;
using RestFoundation.Runtime.Handlers;
using RestFoundation.TypeBinders;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents the default parameter value provider that tries to bind a value
    /// using an associated type binder. If no binder is associated with the parameter
    /// type, a route value is used. If a parameter is named "resource" or is
    /// decorated with the <see cref="ResourceAttribute"/>, a content
    /// formatter is used to bind the data.
    /// </summary>
    public class ParameterValueProvider : IParameterValueProvider
    {
        /// <summary>
        /// Gets the resource parameter name.
        /// </summary>
        protected internal const string ResourceParameterName = "resource";

        /// <summary>
        /// Creates a parameter value based on the routing and HTTP data.
        /// </summary>
        /// <param name="handler">The REST handler associated with the HTTP request.</param>
        /// <param name="parameter">The service method parameters.</param>
        /// <param name="isResource">
        /// true if the parameter represents a REST resource; otherwise, false. Only 1 resource per
        /// service method is allowed.
        /// </param>
        /// <returns>The created parameter value.</returns>
        public virtual object CreateValue(IRestServiceHandler handler, ParameterInfo parameter, out bool isResource)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            IServiceContext context = handler.Context;
            ITypeBinder typeBinder = GetParameterBinder(parameter);

            if (typeBinder is DoNotBindAttribute)
            {
                isResource = false;
                return null;
            }

            if (typeBinder != null)
            {
                isResource = IsResourceParameter(context, parameter);
                return GetTypeBindedValue(context, parameter, typeBinder);
            }

            object routeValue = TryGetRouteValue(parameter, context.Request.RouteValues);

            if (routeValue != null)
            {
                isResource = false;
                return routeValue;
            }

            if (IsResourceParameter(context, parameter))
            {
                isResource = true;
                return GetResourceValue(handler, parameter);
            }

            isResource = false;

            if (typeof(IRestContext).IsAssignableFrom(parameter.ParameterType))
            {
                return Rest.Configuration.ServiceLocator.GetService(parameter.ParameterType);
            }

            return null;
        }

        /// <summary>
        /// Gets a value from the associated <see cref="ITypeBinder"/> object.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="parameter">The service method parameter.</param>
        /// <param name="typeBinder">The associated type binder.</param>
        /// <returns>The parameter value.</returns>
        protected virtual object GetTypeBindedValue(IServiceContext context, ParameterInfo parameter, ITypeBinder typeBinder)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            if (typeBinder == null)
            {
                throw new ArgumentNullException("typeBinder");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            object value = typeBinder.Bind(parameter.Name, parameter.ParameterType, context);

            if (value == null && parameter.DefaultValue != DBNull.Value)
            {
                return parameter.DefaultValue;
            }

            return value;
        }

        /// <summary>
        /// Returns a route value for the service method parameter or null.
        /// </summary>
        /// <param name="parameter">The service method parameter.</param>
        /// <param name="routeValues">The collection of route values.</param>
        /// <returns>The route value or null.</returns>
        protected virtual object TryGetRouteValue(ParameterInfo parameter, IRouteValueCollection routeValues)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            if (routeValues == null)
            {
                throw new ArgumentNullException("routeValues");
            }

            object routeValue = routeValues.TryGet(parameter.Name);

            if (routeValue == null)
            {
                return null;
            }

            object value;

            if (!SafeConvert.TryChangeType(routeValue, parameter.ParameterType, out value))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, Resources.Global.NotFound);
            }

            return value;
        }

        /// <summary>
        /// Gets the resource value for the service method parameter.
        /// </summary>
        /// <param name="handler">The REST handler.</param>
        /// <param name="parameter">The service method parameter.</param>
        /// <returns>The resource value.</returns>
        protected virtual object GetResourceValue(IRestServiceHandler handler, ParameterInfo parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            string contentType = handler.Context.Request.Headers.ContentType;

            if (String.IsNullOrEmpty(contentType))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType, Resources.Global.MissingOrInvalidContentType);
            }

            IMediaTypeFormatter formatter = MediaTypeFormatterRegistry.GetHandlerFormatter(handler, contentType) ??
                                            MediaTypeFormatterRegistry.GetFormatter(contentType);

            if (formatter == null || !formatter.CanFormatRequest)
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType, Resources.Global.MissingOrInvalidContentType);
            }

            object argumentValue;

            try
            {
                argumentValue = formatter.FormatRequest(handler.Context, parameter.ParameterType);

                if (argumentValue != null)
                {
                    LogRequest(handler.Context);
                }
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

                throw new HttpResponseException(HttpStatusCode.BadRequest, Resources.Global.InvalidResourceBody);
            }

            return argumentValue;
        }

        private static ITypeBinder GetParameterBinder(ParameterInfo parameter)
        {
            try
            {
                var typeBinderAttribute = parameter.GetCustomAttribute<TypeBinderAttribute>(false);

                return typeBinderAttribute ?? TypeBinderRegistry.GetBinder(parameter.ParameterType);
            }
            catch (AmbiguousMatchException)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, String.Format(CultureInfo.InvariantCulture,
                                                                                                  Resources.Global.MultipleTypeBindersPerParameter,
                                                                                                  parameter.Name,
                                                                                                  parameter.Member.Name,
                                                                                                  parameter.Member.DeclaringType != null ? parameter.Member.DeclaringType.Name : String.Empty));
            }
        }

        private static bool IsResourceParameter(IServiceContext context, ParameterInfo parameter)
        {
            if (context.Request.Method != HttpMethod.Post && context.Request.Method != HttpMethod.Put && context.Request.Method != HttpMethod.Patch)
            {
                return false;
            }

            if (String.Equals(ResourceParameterName, parameter.Name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (Attribute.IsDefined(parameter, typeof(ResourceAttribute), false))
            {
                return true;
            }

            if (parameter.ParameterType == typeof(IUploadedFile) || parameter.ParameterType == typeof(IEnumerable<IUploadedFile>) ||
                parameter.ParameterType.GetInterface(typeof(IEnumerable<IUploadedFile>).Name) != null)
            {
                return true;
            }

            return false;
        }

        private static void LogRequest(IServiceContext context)
        {
            if (!LogUtility.CanLog)
            {
                return;
            }

            long bodyStreamPosition = context.Request.Body.Position;

            var bodyData = new byte[context.Request.Body.Length];
            context.Request.Body.Seek(0, SeekOrigin.Begin);
            context.Request.Body.Read(bodyData, 0, bodyData.Length);
            context.Request.Body.Seek(bodyStreamPosition, SeekOrigin.Begin);

            string body = context.Request.Headers.ContentCharsetEncoding.GetString(bodyData);
            LogUtility.LogRequestBody(body, context.Request.Headers.ContentType);
        }
    }
}
