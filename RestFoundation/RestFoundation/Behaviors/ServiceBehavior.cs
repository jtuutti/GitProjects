using System;
using System.Reflection;

namespace RestFoundation.Behaviors
{
    public abstract class ServiceBehavior : IServiceBehavior
    {
        public IServiceContext Context { get; set; }
        public IHttpRequest Request { get; set; }
        public IHttpResponse Response { get; set; }

        public virtual bool OnMethodExecuting(object service, MethodInfo method, object resource)
        {
            return true;
        }

        public virtual void OnMethodExecuted(object service, MethodInfo method, object result)
        {
        }

        public virtual bool OnMethodException(object service, MethodInfo method, Exception ex)
        {
            return true;
        }
    }
}
