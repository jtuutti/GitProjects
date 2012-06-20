using System;

namespace RestFoundation.Runtime
{
    public class ServiceFactory : IServiceFactory
    {
        public virtual object Create(Type serviceContractType)
        {
            return ObjectActivator.Create(serviceContractType);
        }
    }
}
