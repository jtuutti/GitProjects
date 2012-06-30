using RestFoundation;
using RestFoundation.ServiceProxy;

namespace RestTestContracts
{
    public interface IDynamicService
    {
        [Url("", HttpMethod.Post)]
        [ProxyOperationDescription("Makes use of dynamically typed resource capabilities")]
        dynamic Post(dynamic resource);
    }
}
