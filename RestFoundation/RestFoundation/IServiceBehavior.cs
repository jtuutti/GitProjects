using System;
using System.Collections.Generic;
using System.Reflection;

namespace RestFoundation
{
    /// <summary>
    /// Defines a service behavior.
    /// </summary>
    public interface IServiceBehavior
    {
        /// <summary>
        /// Gets or sets a collection of service method names to apply the behavior to.
        /// If a null or empty collection is returned, the behavior will be applied to all
        /// service methods.
        /// </summary>
        ICollection<string> AffectedMethods { get; set; }

        /// <summary>
        /// Called before a service method is executed.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="service">The service object.</param>
        /// <param name="method">The service method.</param>
        /// <param name="resource">The resource parameter value, if applicable, or null.</param>
        /// <returns>true to execute the service method; false to stop the request.</returns>
        bool OnMethodExecuting(IServiceContext context, object service, MethodInfo method, object resource);

        /// <summary>
        /// Called after a service method is executed.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="service">The service object.</param>
        /// <param name="method">The service method.</param>
        /// <param name="returnedObj">The service method returned object.</param>
        void OnMethodExecuted(IServiceContext context, object service, MethodInfo method, object returnedObj);

        /// <summary>
        /// Called if an exception occurs during the service method execution.
        /// This method does not catch <see cref="HttpResponseException"/> exceptions because they are
        /// designed to set response status codes and stop the request.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="service">The service object.</param>
        /// <param name="method">The service method.</param>
        /// <param name="ex">The exception.</param>
        /// <returns>true for the exception to bubble up, false to handle the exception and return null.</returns>
        bool OnMethodException(IServiceContext context, object service, MethodInfo method, Exception ex);
    }
}
