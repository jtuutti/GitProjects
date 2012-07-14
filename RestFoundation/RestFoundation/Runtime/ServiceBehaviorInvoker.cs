using System;
using System.Collections.Generic;
using System.Reflection;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents the service behavior invoker class.
    /// </summary>
    public class ServiceBehaviorInvoker
    {
        private readonly IServiceContext m_context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBehaviorInvoker"/> class.
        /// </summary>
        /// <param name="context">The service context.</param>
        public ServiceBehaviorInvoker(IServiceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            m_context = context;
        }

        /// <summary>
        /// Executes <see cref="ISecureServiceBehavior"/> behaviors.
        /// </summary>
        /// <param name="behaviors">The list of behaviors.</param>
        /// <param name="service">The service instance.</param>
        /// <param name="method">The service method.</param>
        public virtual void PerformOnAuthorizingBehaviors(IList<ISecureServiceBehavior> behaviors, object service, MethodInfo method)
        {
            if (behaviors == null) throw new ArgumentNullException("behaviors");
            if (service == null) throw new ArgumentNullException("service");
            if (method == null) throw new ArgumentNullException("method");

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
        /// <returns>true to execute the service method; false to stop the request.</returns>
        public virtual bool PerformOnExecutingBehaviors(IList<IServiceBehavior> behaviors, object service, MethodInfo method, object resource)
        {
            if (behaviors == null) throw new ArgumentNullException("behaviors");
            if (service == null) throw new ArgumentNullException("service");
            if (method == null) throw new ArgumentNullException("method");

            for (int i = 0; i < behaviors.Count; i++)
            {
                if (!behaviors[i].OnMethodExecuting(m_context, service, method, resource))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Executes <see cref="IServiceBehavior"/> behaviors after a service method has been called.
        /// </summary>
        /// <param name="behaviors">The list of behaviors.</param>
        /// <param name="service">The service instance.</param>
        /// <param name="method">The service method.</param>
        /// <param name="returnedObj">The service method returned object.</param>
        public virtual void PerformOnExecutedBehaviors(IList<IServiceBehavior> behaviors, object service, MethodInfo method, object returnedObj)
        {
            if (behaviors == null) throw new ArgumentNullException("behaviors");
            if (service == null) throw new ArgumentNullException("service");
            if (method == null) throw new ArgumentNullException("method");

            for (int i = behaviors.Count - 1; i >= 0; i--)
            {
                behaviors[i].OnMethodExecuted(m_context, service, method, returnedObj);
            }
        }

        /// <summary>
        /// Executes <see cref="IServiceBehavior"/> behaviors when a service method exception has occurred.
        /// </summary>
        /// <param name="behaviors">The list of behaviors.</param>
        /// <param name="service">The service instance.</param>
        /// <param name="method">The service method.</param>
        /// <param name="ex">The exception.</param>
        /// <returns>true for the exception to bubble up, false to handle the exception and return null.</returns>
        public virtual bool PerformOnExceptionBehaviors(IList<IServiceBehavior> behaviors, object service, MethodInfo method, Exception ex)
        {
            if (behaviors == null) throw new ArgumentNullException("behaviors");
            if (service == null) throw new ArgumentNullException("service");
            if (method == null) throw new ArgumentNullException("method");

            for (int i = 0; i < behaviors.Count; i++)
            {
                if (!behaviors[i].OnMethodException(m_context, service, method, ex))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
