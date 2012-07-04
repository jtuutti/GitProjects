using RestFoundation;
using RestFoundation.ServiceProxy;

namespace RestTestContracts
{
    public interface IDynamicService
    {
        [Url(Url.Root)]
        [ProxyOperationDescription("Makes use of dynamically typed resource capabilities")]
        dynamic Post(dynamic resource);
    }
}
