using System;
using System.Collections.Generic;
using System.Reflection;

namespace RestFoundation.Runtime
{
    public class BehaviorInvoker
    {
        public BehaviorInvoker(object service, MethodInfo method)
        {
            if (service == null) throw new ArgumentNullException("service");
            if (method == null) throw new ArgumentNullException("method");

            Service = service;
            Method = method;
        }

        public object Service { get; protected set; }
        public MethodInfo Method { get; protected set; }

        public void PerformOnBindingBehaviors(IEnumerable<ISecureServiceBehavior> behaviors)
        {
            foreach (ISecureServiceBehavior behavior in behaviors)
            {
                behavior.OnMethodAuthorizing(Service, Method);
            }
        }

        public bool PerformOnExecutingBehaviors(List<IServiceBehavior> behaviors, object resource)
        {
            for (int i = 0; i < behaviors.Count; i++)
            {
                if (!behaviors[i].OnMethodExecuting(Service, Method, resource))
                {
                    return false;
                }
            }

            return true;
        }

        public void PerformOnExecutedBehaviors(List<IServiceBehavior> behaviors, object result)
        {
            for (int i = behaviors.Count - 1; i >= 0; i--)
            {
                behaviors[i].OnMethodExecuted(Service, Method, result);
            }
        }

        public bool PerformOnExceptionBehaviors(List<IServiceBehavior> behaviors, Exception ex)
        {
            for (int i = 0; i < behaviors.Count; i++)
            {
                if (!behaviors[i].OnMethodException(Service, Method, ex))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
