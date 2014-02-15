using RestFoundation;
using RestFoundation.ServiceProxy;
using RestTestContracts.Metadata;

namespace RestTestContracts
{
    /// <summary>
    /// Defines a service that posts a resource of dynamic CLR type.
    /// </summary>
    [ProxyMetadata(typeof(DynamicServiceMetadata))]
    public interface IDynamicService
    {
        /// <summary>
        /// Posts a dynamic resource.
        /// </summary>
        /// <param name="resource">The resource</param>
        /// <param name="request">The HTTP request.</param>
        /// <returns>The response.</returns>
        [Url(Url.Root)]
        dynamic Post(dynamic resource, IHttpRequest request);
    }
}
