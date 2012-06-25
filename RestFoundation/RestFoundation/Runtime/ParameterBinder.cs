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
                return SafeConvert.ChangeType(parameterRoute, parameter.ParameterType);
            }

            if (String.Equals(ResourceParameterName, parameter.Name, StringComparison.OrdinalIgnoreCase))
            {
                isResource = true;
                return BindResource(parameter);
            }

            isResource = false;
            return null;
        }

        private object BindResource(ParameterInfo parameter)
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
