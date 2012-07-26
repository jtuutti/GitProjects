using System;
using System.Reflection;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// The base service behavior class.
    /// </summary>
    public abstract class ServiceBehavior : IServiceBehavior
    {
        /// <summary>
        /// Returns a value indicating whether to apply the behavior to the provided method of the specified
        /// service type.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="method">The service method.</param>
        /// <returns>true to apply the behavior; false to bypass.</returns>
        public virtual bool AppliesTo(Type serviceType, MethodInfo method)
        {
            return true;
        }

        /// <summary>
        /// Called before a service method is executed.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="service">The service object.</param>
        /// <param name="method">The service method.</param>
        /// <param name="resource">The resource parameter value, if applicable, or null.</param>
        /// <returns>A service method action.</returns>
        public virtual BehaviorMethodAction OnMethodExecuting(IServiceContext context, object service, MethodInfo method, object resource)
        {
            return BehaviorMethodAction.Execute;
        }

        /// <summary>
        /// Called after a service method is executed.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="service">The service object.</param>
        /// <param name="method">The service method.</param>
        /// <param name="returnedObj">The service method returned object.</param>
        public virtual void OnMethodExecuted(IServiceContext context, object service, MethodInfo method, object returnedObj)
        {
        }

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
        public virtual BehaviorExceptionAction OnMethodException(IServiceContext context, object service, MethodInfo method, Exception ex)
        {
            return BehaviorExceptionAction.BubbleUp;
        }
    }
}
