using System;
using System.Reflection;

namespace RestFoundation
{
    public interface IServiceBehavior
    {
        bool OnMethodExecuting(IServiceContext context, object service, MethodInfo method, object resource);
        void OnMethodExecuted(IServiceContext context, object service, MethodInfo method, object result);
        bool OnMethodException(IServiceContext context, object service, MethodInfo method, Exception ex);
    }
}
