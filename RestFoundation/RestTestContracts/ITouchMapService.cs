using RestFoundation;
using RestFoundation.ServiceProxy;
using RestTestContracts.Metadata;

namespace RestTestContracts
{
    [ProxyMetadata(typeof(TouchMapServiceMetadata))]
    public interface ITouchMapService
    {
        [Url(Url.Root)]
        object Get();
    }
}
