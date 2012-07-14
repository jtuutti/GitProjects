using System;

namespace RestFoundation.Runtime
{
    public class ServiceFactory : IServiceFactory
    {
        public virtual object Create(Type serviceContractType, IServiceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            return Rest.Active.CreateObject(serviceContractType);
        }
    }
}
