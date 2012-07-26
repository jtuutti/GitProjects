using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents the default HTTP method resolver. It uses the property values of the <see cref="UrlAttribute"/>
    /// as well as the name of the method to resolve the supported HTTP methods.
    /// </summary>
    public class HttpMethodResolver : IHttpMethodResolver
    {
        /// <summary>
        /// Returns a sequence of HTTP methods supported by the service method.
        /// </summary>
        /// <param name="method">The service method.</param>
        /// <returns>A sequence of HTTP methods.</returns>
        /// <exception cref="InvalidOperationException">If the HTTP method could not be resolved.</exception>
        public virtual IEnumerable<HttpMethod> Resolve(MethodInfo method)
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
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
                                                                 "Method '{0}' of the service contract type '{1}' does not have any HTTP methods defined in the UrlAttribute declaration.",
                                                                  method.DeclaringType != null ? method.DeclaringType.Name : "Unknown",
                                                                  method.Name));
            }

            return resolvedMethod.Value == HttpMethod.Get ? new[] { HttpMethod.Get, HttpMethod.Head } : new[] { resolvedMethod.Value };
        }
    }
}
