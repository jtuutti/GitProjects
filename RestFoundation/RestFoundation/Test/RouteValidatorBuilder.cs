using System;
using System.Linq.Expressions;

namespace RestFoundation.Test
{
    public sealed class RouteValidatorBuilder
    {
        private readonly HttpMethod m_httpMethod;
        private readonly string m_relativeUrl;

        internal RouteValidatorBuilder(string relativeUrl, HttpMethod httpMethod)
        {
            m_relativeUrl = relativeUrl;
            m_httpMethod = httpMethod;
        }

        public void Invokes<T>(Expression<Action<T>> serviceMethodDelegate)
        {
            var testRoute = new RouteValidator<T>(m_relativeUrl, m_httpMethod, serviceMethodDelegate);
            testRoute.Validate();
        }
    }
}
