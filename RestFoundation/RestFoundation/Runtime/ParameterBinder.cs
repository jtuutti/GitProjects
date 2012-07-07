using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Web;
using RestFoundation.Collections;
using RestFoundation.DataFormatters;

namespace RestFoundation.Runtime
{
    public class ParameterBinder : IParameterBinder
    {
        protected const string ResourceParameterName = "resource";

        public virtual object BindParameter(IServiceContext context, ParameterInfo parameter, out bool isResource)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (parameter == null) throw new ArgumentNullException("parameter");

            IDataBinder dataBinder = DataBinderRegistry.GetBinder(parameter.ParameterType);

            if (dataBinder != null)
            {
                isResource = false;
                return dataBinder.Bind(context, parameter.Name);
            }

            object routeValue = BindRouteValue(parameter, context.Request.RouteValues);

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
                return BindResourceValue(parameter, context);
            }

            isResource = false;
            return null;
        }

        protected virtual object BindRouteValue(ParameterInfo parameter, IObjectValueCollection routeValues)
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

        protected virtual object BindResourceValue(ParameterInfo parameter, IServiceContext context)
        {
            if (parameter == null) throw new ArgumentNullException("parameter");
            if (context == null) throw new ArgumentNullException("context");

            IDataFormatter formatter = DataFormatterRegistry.GetFormatter(context.Request.Headers.ContentType);

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
                if (ex is HttpResponseException || ex is HttpRequestValidationException)
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
