using System;

namespace RestFoundation.Test
{
    public sealed class HttpMethodBuilder
    {
        private readonly string m_relativeUrl;

        internal HttpMethodBuilder(string relativeUrl)
        {
            m_relativeUrl = relativeUrl;
        }

        public RouteValidatorBuilder WithHttpMethod(HttpMethod httpMethod)
        {
            if (!Enum.IsDefined(typeof(HttpMethod), httpMethod))
            {
                throw new ArgumentOutOfRangeException("httpMethod");
            }

            return new RouteValidatorBuilder(m_relativeUrl, httpMethod);
        }
    }
}
