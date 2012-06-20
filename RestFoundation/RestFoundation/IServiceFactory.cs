using System;

namespace RestFoundation
{
    public interface IServiceFactory
    {
        object Create(Type serviceContractType);
    }
}
