// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.Reflection;
using RestFoundation.Behaviors;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents a service behavior invoker class.
    /// </summary>
    public class ServiceBehaviorInvoker : IServiceBehaviorInvoker
    {
        private readonly IServiceContext m_context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBehaviorInvoker"/> class.
        /// </summary>
        /// <param name="context">The service context.</param>
        public ServiceBehaviorInvoker(IServiceContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            m_context = context;
        }

        /// <summary>
        /// Executes <see cref="ISecureServiceBehavior"/> behaviors.
        /// </summary>
        /// <param name="behaviors">The list of behaviors.</param>
        /// <param name="service">The service instance.</param>
        /// <param name="method">The service method.</param>
        public virtual void InvokeOnAuthorizingBehaviors(IList<ISecureServiceBehavior> behaviors, object service, MethodInfo method)
        {
            if (behaviors == null)
            {
                throw new ArgumentNullException("behaviors");
            }

            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            foreach (ISecureServiceBehavior behavior in behaviors)
            {
                behavior.OnMethodAuthorizing(m_context, service, method);
            }
        }

        /// <summary>
        /// Executes <see cref="IServiceBehavior"/> behaviors before a service method is called.
        /// </summary>
        /// <param name="behaviors">The list of behaviors.</param>
        /// <param name="service">The service instance.</param>
        /// <param name="method">The service method.</param>
        /// <param name="resource">The input resource for the service method.</param>
        /// <returns>A service method action.</returns>
        public virtual BehaviorMethodAction InvokeOnExecutingBehaviors(IList<IServiceBehavior> behaviors, object service, MethodInfo method, object resource)
        {
            if (behaviors == null)
            {
                throw new ArgumentNullException("behaviors");
            }

            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            for (int i = 0; i < behaviors.Count; i++)
            {
                if (behaviors[i].OnMethodExecuting(m_context, service, method, resource) == BehaviorMethodAction.Stop)
                {
                    return BehaviorMethodAction.Stop;
                }
            }

            return BehaviorMethodAction.Execute;
        }

        /// <summary>
        /// Executes <see cref="IServiceBehavior"/> behaviors after a service method has been called.
        /// </summary>
        /// <param name="behaviors">The list of behaviors.</param>
        /// <param name="service">The service instance.</param>
        /// <param name="method">The service method.</param>
        /// <param name="returnedObj">The service method returned object.</param>
        public virtual void InvokeOnExecutedBehaviors(IList<IServiceBehavior> behaviors, object service, MethodInfo method, object returnedObj)
        {
            if (behaviors == null)
            {
                throw new ArgumentNullException("behaviors");
            }

            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            for (int i = behaviors.Count - 1; i >= 0; i--)
            {
                behaviors[i].OnMethodExecuted(m_context, service, method, returnedObj);
            }
        }
    }
}
