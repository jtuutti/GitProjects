using System;

namespace RestFoundation.Runtime
{
    public class ServiceFactory : IServiceFactory
    {
        public virtual object Create(Type serviceContractType)
        {
            return Rest.Active.CreateObject(serviceContractType);
        }
    }
}
