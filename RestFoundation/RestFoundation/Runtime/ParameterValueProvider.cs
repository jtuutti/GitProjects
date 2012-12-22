﻿// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web;
using RestFoundation.Collections;
using RestFoundation.Formatters;
using RestFoundation.Runtime.Handlers;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents the default parameter value provider that tries to bind a value
    /// using an associated type binder. If no binder is associated with the parameter
    /// type, a route value is used. If a parameter is named "resource" or is
    /// decorated with the <see cref="ResourceParameterAttribute"/>, a content
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
        /// <param name="parameter">The service method parameters.</param>
        /// <param name="handler">The REST handler associated with the HTTP request.</param>
        /// <param name="isResource">
        /// true if the parameter represents a REST resource; otherwise, false. Only 1 resource per
        /// service method is allowed.
        /// </param>
        /// <returns>The created parameter value.</returns>
        public virtual object CreateValue(ParameterInfo parameter, IRestHandler handler, out bool isResource)
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
            ITypeBinder typeBinder = TypeBinderRegistry.GetBinder(parameter.ParameterType);

            if (typeBinder != null)
            {
                isResource = false;
                return typeBinder.Bind(parameter.ParameterType, parameter.Name, context);
            }

            object routeValue = TryGetRouteValue(parameter, context.Request.RouteValues);

            if (routeValue != null)
            {
                isResource = false;
                return routeValue;
            }

            if (IsResourceParameter(parameter, context))
            {
                isResource = true;
                return GetResourceValue(parameter, handler);
            }

            isResource = false;
            return null;
        }

        /// <summary>
        /// Returns a route value for the service method parameter or null.
        /// </summary>
        /// <param name="parameter">The service method parameter.</param>
        /// <param name="routeValues">The collection of route values.</param>
        /// <returns>The route value or null.</returns>
        protected virtual object TryGetRouteValue(ParameterInfo parameter, IObjectValueCollection routeValues)
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
                throw new HttpResponseException(HttpStatusCode.NotFound, RestResources.NotFound);
            }

            return value;
        }

        /// <summary>
        /// Gets the resource value for the service method parameter.
        /// </summary>
        /// <param name="parameter">The service method parameter.</param>
        /// <param name="handler">The REST handler.</param>
        /// <returns>The resource value.</returns>
        protected virtual object GetResourceValue(ParameterInfo parameter, IRestHandler handler)
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

            IMediaTypeFormatter formatter = MediaTypeFormatterRegistry.GetHandlerFormatter(handler, contentType) ??
                                            MediaTypeFormatterRegistry.GetFormatter(contentType);

            if (formatter == null || formatter is BlockFormatter)
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType, RestResources.MissingValidMediaType);
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

                throw new HttpResponseException(HttpStatusCode.BadRequest, RestResources.InvalidResourceBody);
            }

            return argumentValue;
        }

        private static void LogRequest(IServiceContext context)
        {
            if (!LogUtility.CanLog)
            {
                return;
            }

            using (var dataStream = new MemoryStream())
            {
                context.Request.Body.Seek(0, SeekOrigin.Begin);
                context.Request.Body.CopyTo(dataStream);
                dataStream.Seek(0, SeekOrigin.Begin);

                var dataReader = new StreamReader(dataStream, context.Request.Headers.ContentCharsetEncoding);
                LogUtility.LogRequestBody(dataReader.ReadToEnd(), context.Request.Headers.ContentType);
            }
        }

        private static bool IsResourceParameter(ParameterInfo parameter, IServiceContext context)
        {
            if (context.Request.Method != HttpMethod.Post && context.Request.Method != HttpMethod.Put && context.Request.Method != HttpMethod.Patch)
            {
                return false;
            }

            if (String.Equals(ResourceParameterName, parameter.Name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (Attribute.IsDefined(parameter, typeof(ResourceParameterAttribute), false))
            {
                return true;
            }

            if (parameter.ParameterType == typeof(IEnumerable<IUploadedFile>) || parameter.ParameterType == typeof(ICollection<IUploadedFile>))
            {
                return true;
            }

            return false;
        }
    }
}
