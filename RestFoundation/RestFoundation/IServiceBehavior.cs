using System.Reflection;

namespace RestFoundation
{
    public interface IServiceBehavior
    {
        IHttpRequest Request { get; set; }
        IHttpResponse Response { get; set; }

        void OnActionBinding(object service, MethodInfo actionMethod);
        bool OnActionExecuting(object resource);
        void OnActionExecuted(object result);
    }
}
