// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Reflection;

namespace RestFoundation
{
    /// <summary>
    /// Returns the HTTP methods that are supported by the provided service method.
    /// If no HTTP methods were specified in the <see cref="UrlAttribute"/> properties,
    /// it tries to figure out the HTTP method based on the service method name.
    /// </summary>
    public interface IHttpMethodResolver
    {
        /// <summary>
        /// Returns a sequence of HTTP methods supported by the service method.
        /// </summary>
        /// <param name="method">The service method.</param>
        /// <returns>A sequence of HTTP methods.</returns>
        /// <exception cref="InvalidOperationException">If the HTTP method could not be resolved.</exception>
        IEnumerable<HttpMethod> Resolve(MethodInfo method);
    }
}
