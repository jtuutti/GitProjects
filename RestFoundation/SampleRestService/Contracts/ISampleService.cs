using System.Linq;
using System.Threading.Tasks;
using RestFoundation;
using RestFoundation.Behaviors;
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
        Product GetById([Constraint(Constraint.UnsignedInteger)] int id);

        [Url("products")]
        IQueryable<Product> GetAll();

        [Url("product"), AssertValidation(true)]
        Task<Product> PostAsync(Product resource);

        [Url("product/{id}"), AssertValidation(true)]
        Task<Product> PutAsync([Constraint(Constraint.UnsignedInteger)] int id, Product resource);

        [Url("product/{id}/in-stock/{inStock}")]
        Product PatchStockStatus([Constraint(Constraint.UnsignedInteger)] int id, bool inStock = true);

        [Url("product/{id}")]
        StatusCodeResult DeleteById([Constraint(Constraint.UnsignedInteger)] int id);
    }
}
