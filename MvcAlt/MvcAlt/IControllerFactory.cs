using System;

namespace MvcAlt
{
    public interface IControllerFactory
    {
        Type GetHandler(IHttpRequest request);
    }
}
