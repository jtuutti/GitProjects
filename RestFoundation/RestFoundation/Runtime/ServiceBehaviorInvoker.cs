using System;
using System.Collections.Generic;
using System.Reflection;

namespace RestFoundation.Runtime
{
    public class ServiceBehaviorInvoker
    {
        private readonly IServiceContext m_context;

        public ServiceBehaviorInvoker(IServiceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            m_context = context;
        }

        public virtual void PerformOnBindingBehaviors(IList<ISecureServiceBehavior> behaviors, object service, MethodInfo method)
        {
            if (behaviors == null) throw new ArgumentNullException("behaviors");
            if (service == null) throw new ArgumentNullException("service");
            if (method == null) throw new ArgumentNullException("method");

            foreach (ISecureServiceBehavior behavior in behaviors)
            {
                behavior.OnMethodAuthorizing(m_context, service, method);
            }
        }

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

        public virtual void PerformOnExecutedBehaviors(IList<IServiceBehavior> behaviors, object service, MethodInfo method, object result)
        {
            if (behaviors == null) throw new ArgumentNullException("behaviors");
            if (service == null) throw new ArgumentNullException("service");
            if (method == null) throw new ArgumentNullException("method");

            for (int i = behaviors.Count - 1; i >= 0; i--)
            {
                behaviors[i].OnMethodExecuted(m_context, service, method, result);
            }
        }

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
