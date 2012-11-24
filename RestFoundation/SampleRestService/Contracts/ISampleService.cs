using System.Linq;
using RestFoundation;
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

        [Url("product")]
        Product Post(Product resource);

        [Url("product/{id}")]
        Product Put(int id, Product resource);

        [Url("product/{id}/in-stock/{inStock}")]
        Product PatchStockStatus(int id, bool? inStock);

        [Url("product/{id}")]
        StatusResult DeleteById(int id);
    }
}
