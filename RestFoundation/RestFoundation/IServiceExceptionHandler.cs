using System;
using System.Reflection;

namespace RestFoundation
{
    /// <summary>
    /// Defines a service exception handler.
    /// </summary>
    public interface IServiceExceptionHandler
    {
        /// <summary>
        /// Called if an exception occurs during the service method execution.
        /// This method does not catch <see cref="HttpResponseException"/> exceptions because they are
        /// designed to set response status codes and stop the request.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="service">The service object.</param>
        /// <param name="method">The service method.</param>
        /// <param name="ex">The exception.</param>
        /// <returns>A service method exception action.</returns>
        ExceptionAction Handle(IServiceContext context, object service, MethodInfo method, Exception ex);
    }
}
