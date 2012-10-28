// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Reflection;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// The base service behavior class.
    /// This class cannot be instantiated.
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
        /// <param name="serviceContext">The service context.</param>
        /// <param name="behaviorContext">The "method executing" behavior context.</param>
        /// <returns>A service method action.</returns>
        public virtual BehaviorMethodAction OnMethodExecuting(IServiceContext serviceContext, MethodExecutingContext behaviorContext)
        {
            return BehaviorMethodAction.Execute;
        }

        /// <summary>
        /// Called after a service method is executed.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="behaviorContext">The "method executed" behavior context.</param>
        public virtual void OnMethodExecuted(IServiceContext serviceContext, MethodExecutedContext behaviorContext)
        {
        }
    }
}
