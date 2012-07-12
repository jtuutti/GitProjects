using System;
using System.Collections.Generic;
using System.Reflection;

namespace RestFoundation.Behaviors
{
    public class ResponseHeaderBehavior : ServiceBehavior
    {
        private readonly IDictionary<string, string> m_headers;

        public ResponseHeaderBehavior(string headerName, string headerValue)
        {
            if (String.IsNullOrEmpty(headerName)) throw new ArgumentNullException("headerName");
            if (String.IsNullOrEmpty(headerValue)) throw new ArgumentNullException("headerValue");

            m_headers = new Dictionary<string, string>
            {
                { headerName, headerValue }
            };
        }

        public ResponseHeaderBehavior(IDictionary<string, string> headers)
        {
            if (headers == null) throw new ArgumentNullException("headers");

            m_headers = new SortedDictionary<string, string>(headers, StringComparer.OrdinalIgnoreCase);
        }

        public override void OnMethodExecuted(IServiceContext context, object service, MethodInfo method, object result)
        {
            if (context == null) throw new ArgumentNullException("context");

            foreach (string headerName in m_headers.Keys)
            {
                string headerValue;

                if (m_headers.TryGetValue(headerName, out headerValue) && !String.IsNullOrEmpty(headerValue))
                {
                    context.Response.SetHeader(headerName, headerValue);
                }
            }
        }
    }
}
