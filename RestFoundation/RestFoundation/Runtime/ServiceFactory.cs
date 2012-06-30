using System;

namespace RestFoundation.Runtime
{
    public class ServiceFactory : IServiceFactory
    {
        public virtual object Create(IServiceContext context, Type serviceContractType)
        {
            if (context == null) throw new ArgumentNullException("context");

            return Rest.Active.CreateObject(serviceContractType);
        }
    }
}
