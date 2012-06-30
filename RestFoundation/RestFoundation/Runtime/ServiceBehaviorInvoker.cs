using System;
using System.Collections.Generic;
using System.Reflection;

namespace RestFoundation.Runtime
{
    public sealed class ServiceBehaviorInvoker
    {
        internal ServiceBehaviorInvoker(IServiceContext context, object service, MethodInfo method)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (service == null) throw new ArgumentNullException("service");
            if (method == null) throw new ArgumentNullException("method");

            Context = context;
            Service = service;
            Method = method;
        }

        public IServiceContext Context { get; private set; }
        public object Service { get; private set; }
        public MethodInfo Method { get; private set; }

        public void PerformOnBindingBehaviors(IList<ISecureServiceBehavior> behaviors)
        {
            foreach (ISecureServiceBehavior behavior in behaviors)
            {
                behavior.OnMethodAuthorizing(Context, Service, Method);
            }
        }

        public bool PerformOnExecutingBehaviors(IList<IServiceBehavior> behaviors, object resource)
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

        public void PerformOnExecutedBehaviors(IList<IServiceBehavior> behaviors, object result)
        {
            for (int i = behaviors.Count - 1; i >= 0; i--)
            {
                behaviors[i].OnMethodExecuted(Context, Service, Method, result);
            }
        }

        public bool PerformOnExceptionBehaviors(IList<IServiceBehavior> behaviors, Exception ex)
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
