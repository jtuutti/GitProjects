using System;

namespace MvcAlt
{
    public interface IActionMethodResolver
    {
        Delegate Resolve(IHttpRequest request);
    }
}
