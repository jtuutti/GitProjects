using System;
using System.Collections.Generic;
using System.Reflection;

namespace RestFoundation
{
    public interface IServiceBehavior
    {
        ICollection<string> AffectedMethods { get; }

        bool OnMethodExecuting(IServiceContext context, object service, MethodInfo method, object resource);
        void OnMethodExecuted(IServiceContext context, object service, MethodInfo method, object result);
        bool OnMethodException(IServiceContext context, object service, MethodInfo method, Exception ex);
    }
}
