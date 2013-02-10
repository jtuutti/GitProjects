// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System.Reflection;
using RestFoundation.Runtime.Handlers;

namespace RestFoundation
{
    /// <summary>
    /// Defines a service method invoker.
    /// </summary>
    public interface IServiceMethodInvoker
    {
        /// <summary>
        /// Invokes the service method.
        /// </summary>
        /// <param name="service">The service instance.</param>
        /// <param name="method">The service method.</param>
        /// <param name="handler">The REST handler associated with the HTTP request.</param>
        /// <returns>The return value of the executed service method.</returns>
        object Invoke(object service, MethodInfo method, IRestServiceHandler handler);
    }
}
