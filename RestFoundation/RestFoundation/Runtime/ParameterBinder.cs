using System;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Web;
using RestFoundation.DataFormatters;

namespace RestFoundation.Runtime
{
    public class ParameterBinder : IParameterBinder
    {
        protected const string ResourceParameterName = "resource";

        private readonly IHttpRequest m_request;

        public ParameterBinder(IHttpRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");

            m_request = request;
        }

        public object BindParameter(ParameterInfo parameter, out bool isResource)
        {
            object parameterRoute = m_request.RouteValues.TryGet(parameter.Name);

            if (parameterRoute != null)
            {
                isResource = false;
                return GetParameterValue(parameter, parameterRoute);
            }

            if (String.Equals(ResourceParameterName, parameter.Name, StringComparison.OrdinalIgnoreCase) ||
                Attribute.GetCustomAttribute(parameter, typeof(BindResourceAttribute), true) != null)
            {
                isResource = true;
                return GetResourceValue(parameter);
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

            var constraintAttribute = Attribute.GetCustomAttribute(parameter, typeof(ParameterConstraintAttribute), false) as ParameterConstraintAttribute;

            if (constraintAttribute != null && !constraintAttribute.Pattern.IsMatch(Convert.ToString(parameterRoute, CultureInfo.InvariantCulture)))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, "Not Found");
            }

            return value;
        }

        private object GetResourceValue(ParameterInfo parameter)
        {
            IDataFormatter formatter = DataFormatterRegistry.GetFormatter(parameter.ParameterType) ??
                                       DataFormatterRegistry.GetFormatter(m_request.Headers.ContentType);

            if (formatter == null)
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType, "Unsupported content type provided");
            }

            object argumentValue;

            try
            {
                argumentValue = formatter.Format(m_request.Body, m_request.Headers.ContentCharsetEncoding, parameter.ParameterType);
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
