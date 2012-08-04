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
        /// <param name="method">The service method.</param>
        /// <param name="service">The service instance.</param>
        /// <param name="handler">The REST handler associated with the HTTP request.</param>
        /// <returns>The return value of the executed service method.</returns>
        object Invoke(MethodInfo method, object service, IRestHandler handler);
    }
}
