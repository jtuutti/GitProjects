using System;
using System.Net;
using System.Reflection;
using System.Web;
using RestFoundation.DataFormatters;

namespace RestFoundation.Runtime
{
    public class ParameterBinder : IParameterBinder
    {
        protected const string ResourceParameterName = "resource";

        public virtual object BindParameter(IServiceContext context, ParameterInfo parameter, out bool isResource)
        {
            if (context == null) throw new ArgumentNullException("context");

            IDataBinder dataBinder = DataBinderRegistry.GetBinder(parameter.ParameterType);

            if (dataBinder != null)
            {
                isResource = false;
                return dataBinder.Bind(context, parameter.Name);
            }

            object parameterRoute = context.Request.RouteValues.TryGet(parameter.Name);

            if (parameterRoute != null)
            {
                isResource = false;
                return GetParameterValue(parameter, parameterRoute);
            }

            if ((context.Request.Method == HttpMethod.Post || context.Request.Method == HttpMethod.Put || context.Request.Method == HttpMethod.Patch) &&
                String.Equals(ResourceParameterName, parameter.Name, StringComparison.OrdinalIgnoreCase) ||
                Attribute.GetCustomAttribute(parameter, typeof(BindResourceAttribute), true) != null)
            {
                isResource = true;
                return GetResourceValue(parameter, context);
            }

            isResource = false;
            return null;
        }

        private static object GetParameterValue(ParameterInfo parameter, object parameterRoute)
        {
            object value;

            if (!SafeConvert.TryChangeType(parameterRoute, parameter.ParameterType, out value))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, "Not Found");
            }

            return value;
        }

        private static object GetResourceValue(ParameterInfo parameter, IServiceContext context)
        {
            IDataFormatter formatter = DataFormatterRegistry.GetFormatter(parameter.ParameterType) ??
                                       DataFormatterRegistry.GetFormatter(context.Request.Headers.ContentType);

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
                if (ex is HttpRequestValidationException)
                {
                    throw;
                }

                throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid resource body provided");
            }

            return argumentValue;
        }
    }
}
