using RestFoundation;
using RestFoundation.ServiceProxy;
using RestTestContracts.Behaviors;
using RestTestContracts.Metadata;

namespace RestTestContracts
{
    [ProxyMetadata(typeof(TouchMapServiceMetadata))]
    public interface ITouchMapService
    {
        [Url(Url.Root)]
        [T3ContextBehavior]
        object Get();
    }
}
