using System;
using System.Collections.Generic;
using System.Reflection;

namespace RestFoundation.Behaviors
{
    public abstract class ServiceBehavior : IServiceBehavior
    {
        public virtual ICollection<string> AffectedMethods
        {
            get
            {
                return new string[0];
            }
        }

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
