using RestFoundation;
using RestFoundation.ServiceProxy;
using RestTestContracts.Metadata;

namespace RestTestContracts
{
    [ProxyMetadata(typeof(DynamicServiceMetadata))]
    public interface IDynamicService
    {
        [Url(Url.Root)]
        dynamic Post(dynamic resource);
    }
}
