using System;
using System.Net;

namespace RestFoundation.ServiceProxy.Attributes
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

        internal string Condition { get; private set; }
        internal HttpStatusCode StatusCode { get; private set; }
    }
}
