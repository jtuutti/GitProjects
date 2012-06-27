using System;
using System.Net;

namespace RestFoundation.ServiceProxy.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class ProxyStatusCodeAttribute : Attribute
    {
        private readonly HttpStatusCode m_statusCode;
        private readonly string m_condition;

        public ProxyStatusCodeAttribute(HttpStatusCode statusCode, string condition)
        {
            if (String.IsNullOrEmpty(condition))
            {
                throw new ArgumentNullException("condition");
            }

            m_statusCode = statusCode;
            m_condition = condition;
        }

        public string Condition
        {
            get
            {
                return m_condition;
            }
        }

        public HttpStatusCode StatusCode
        {
            get
            {
                return m_statusCode;
            }
        }
    }
}
