using System.Linq;
using RestFoundation;
using RestFoundation.Behaviors.Attributes;
using RestFoundation.Results;
using RestFoundation.ServiceProxy;
using SampleRestService.Contracts.Metadata;
using SampleRestService.Resources;

namespace SampleRestService.Contracts
{
    [ProxyMetadata(typeof(SampleServiceMetadata))]
    public interface ISampleService
    {
        [Url("product/{id}")]
        Product GetById([ParameterConstraint(@"\d+")] int id);

        [Url("products")]
        IQueryable<Product> GetAll();

        [Url("product"), ValidateResource(true)]
        Product Post(Product resource);

        [Url("product/{id}"), ValidateResource(true)]
        Product Put(int id, Product resource);

        [Url("product/{id}/in-stock/{inStock}")]
        Product PatchStockStatus(int id, bool inStock = true);

        [Url("product/{id}")]
        StatusResult DeleteById(int id);
    }
}
