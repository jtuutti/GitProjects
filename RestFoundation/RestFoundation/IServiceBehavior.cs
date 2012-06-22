using System;
using System.Reflection;

namespace RestFoundation
{
    public interface IServiceBehavior
    {
        IServiceContext Context { get; set; }
        IHttpRequest Request { get; set; }
        IHttpResponse Response { get; set; }

        bool OnMethodExecuting(object service, MethodInfo method, object resource);
        void OnMethodExecuted(object service, MethodInfo method, object result);
        bool OnMethodException(object service, MethodInfo method, Exception ex);
    }
}
