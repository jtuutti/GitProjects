using System;
using System.Reflection;

namespace RestFoundation.Behaviors
{
    public abstract class ServiceBehavior : IServiceBehavior
    {
        public virtual bool OnMethodExecuting(IServiceContext context, object service, MethodInfo method, object resource)
        {
            return true;
        }

        public virtual void OnMethodExecuted(IServiceContext context, object service, MethodInfo method, object result)
        {
        }

        public virtual bool OnMethodException(IServiceContext context, object service, MethodInfo method, Exception ex)
        {
            return true;
        }
    }
}
