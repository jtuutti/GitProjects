using System;

namespace RestFoundation
{
    public interface IServiceFactory
    {
        object Create(IServiceContext context, Type serviceContractType);
    }
}
