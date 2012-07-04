using System;
using System.Collections.Generic;
using System.Reflection;

namespace RestFoundation.Runtime
{
    public class HttpMethodResolver : IHttpMethodResolver
    {
        public IEnumerable<HttpMethod> Resolve(MethodInfo method)
        {
            if (method == null) throw new ArgumentNullException("method");

            HttpMethod? resolvedMethod = null;

            foreach (HttpMethod httpMethod in Enum.GetValues(typeof(HttpMethod)))
            {
                if (method.Name.StartsWith(httpMethod.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    resolvedMethod = httpMethod;
                    break;
                }
            }

            if (!resolvedMethod.HasValue || resolvedMethod.Value == HttpMethod.Options)
            {
                throw new InvalidOperationException(String.Format("Method '{0}' of the service contract type '{1}' does not have any HTTP methods defined in the UrlAttribute declaration.",
                                                                  method.DeclaringType != null ? method.DeclaringType.Name : "Unknown",
                                                                  method.Name));
            }

            return resolvedMethod.Value == HttpMethod.Get ? new[] { HttpMethod.Get, HttpMethod.Head } : new[] { resolvedMethod.Value };
        }
    }
}
