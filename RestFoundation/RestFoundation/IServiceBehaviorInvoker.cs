using System.Collections.Generic;
using System.Reflection;
using RestFoundation.Behaviors;

namespace RestFoundation
{
    /// <summary>
    /// Defines a service behavior invoker.
    /// </summary>
    public interface IServiceBehaviorInvoker
    {
        /// <summary>
        /// Invokes <see cref="ISecureServiceBehavior"/> behaviors.
        /// </summary>
        /// <param name="behaviors">The list of behaviors.</param>
        /// <param name="service">The service instance.</param>
        /// <param name="method">The service method.</param>
        void InvokeOnAuthorizingBehaviors(IList<ISecureServiceBehavior> behaviors, object service, MethodInfo method);

        /// <summary>
        /// Invokes <see cref="IServiceBehavior"/> behaviors before a service method is called.
        /// </summary>
        /// <param name="behaviors">The list of behaviors.</param>
        /// <param name="service">The service instance.</param>
        /// <param name="method">The service method.</param>
        /// <param name="resource">The input resource for the service method.</param>
        /// <returns>A service method action.</returns>
        BehaviorMethodAction InvokeOnExecutingBehaviors(IList<IServiceBehavior> behaviors, object service, MethodInfo method, object resource);

        /// <summary>
        /// Invokes <see cref="IServiceBehavior"/> behaviors after a service method has been called.
        /// </summary>
        /// <param name="behaviors">The list of behaviors.</param>
        /// <param name="service">The service instance.</param>
        /// <param name="method">The service method.</param>
        /// <param name="returnedObj">The service method returned object.</param>
        void InvokeOnExecutedBehaviors(IList<IServiceBehavior> behaviors, object service, MethodInfo method, object returnedObj);
    }
}
