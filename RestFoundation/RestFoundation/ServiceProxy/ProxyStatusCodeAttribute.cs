using System;
using System.Net;

namespace RestFoundation.ServiceProxy
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class ProxyStatusCodeAttribute : Attribute
    {
        public ProxyStatusCodeAttribute(HttpStatusCode statusCode, string condition)
        {
            if (String.IsNullOrEmpty(condition))
            {
                throw new ArgumentNullException("condition");
            }

            StatusCode = statusCode;
            Condition = condition;
        }

        public string Condition { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
    }
}
