using RestFoundation;
using RestFoundation.ServiceProxy;
using RestTestContracts.Behaviors;
using RestTestContracts.Metadata;

namespace RestTestContracts
{
    /// <summary>
    /// Defines the touch map service.
    /// </summary>
    [ProxyMetadata(typeof(TouchMapServiceMetadata))]
    public interface ITouchMapService
    {
        /// <summary>
        /// Returns the touch map resource.
        /// </summary>
        /// <returns>The touch map resource.</returns>
        [Url(Url.Root)]
        [T3ContextBehavior]
        object Get();
    }
}
