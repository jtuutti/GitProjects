using System.Reflection;

namespace RestFoundation.Behaviors
{
    public abstract class ServiceBehavior : IServiceBehavior
    {
        public IHttpRequest Request { get; set; }
        public IHttpResponse Response { get; set; }

        public virtual void OnActionBinding(object service, MethodInfo actionMethod)
        {
        }

        public virtual bool OnActionExecuting(object resource)
        {
            return true;
        }

        public virtual void OnActionExecuted(object result)
        {
        }
    }
}
