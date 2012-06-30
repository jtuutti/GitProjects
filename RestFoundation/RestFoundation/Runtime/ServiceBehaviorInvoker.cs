using System;
using System.Collections.Generic;
using System.Reflection;

namespace RestFoundation.Runtime
{
    public class ServiceBehaviorInvoker
    {
        public ServiceBehaviorInvoker(IServiceContext context, object service, MethodInfo method)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (service == null) throw new ArgumentNullException("service");
            if (method == null) throw new ArgumentNullException("method");

            Context = context;
            Service = service;
            Method = method;
        }

        public IServiceContext Context { get; protected set; }
        public object Service { get; protected set; }
        public MethodInfo Method { get; protected set; }

        public virtual void PerformOnBindingBehaviors(IEnumerable<ISecureServiceBehavior> behaviors)
        {
            foreach (ISecureServiceBehavior behavior in behaviors)
            {
                behavior.OnMethodAuthorizing(Context, Service, Method);
            }
        }

        public virtual bool PerformOnExecutingBehaviors(List<IServiceBehavior> behaviors, object resource)
        {
            for (int i = 0; i < behaviors.Count; i++)
            {
                if (!behaviors[i].OnMethodExecuting(Context, Service, Method, resource))
                {
                    return false;
                }
            }

            return true;
        }

        public virtual void PerformOnExecutedBehaviors(List<IServiceBehavior> behaviors, object result)
        {
            for (int i = behaviors.Count - 1; i >= 0; i--)
            {
                behaviors[i].OnMethodExecuted(Context, Service, Method, result);
            }
        }

        public virtual bool PerformOnExceptionBehaviors(List<IServiceBehavior> behaviors, Exception ex)
        {
            for (int i = 0; i < behaviors.Count; i++)
            {
                if (!behaviors[i].OnMethodException(Context, Service, Method, ex))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
