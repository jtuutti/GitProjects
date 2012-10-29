using System.Linq;
using System.Net;
using RestFoundation;
using RestFoundation.Results;
using RestFoundation.ServiceProxy;
using SampleRestService.Resources;
using SampleRestService.Resources.ExampleBuilders;

namespace SampleRestService.Contracts
{
    public interface ISampleService
    {
        [Url("product/{id}")]
        [ProxyOperationDescription("Get a product by its ID")]
        [ProxyResourceExample(RequestBuilderType = typeof(ProductExampleBuilder))]
        Product GetById([ParameterConstraint(@"\d+"), ProxyRouteParameter(1)] int id);

        [Url("products")]
        [ProxyOperationDescription("Gets all products")]
        [ProxyResourceExample(ResponseBuilderType = typeof(ProductArrayExampleBuilder))]
        IQueryable<Product> GetAll();

        [Url("product")]
        [ProxyOperationDescription("Creates a new product")]
        [ProxyStatusCode(HttpStatusCode.Created, "Product created")]
        [ProxyResourceExample(RequestBuilderType = typeof(ProductExampleBuilder), ResponseBuilderType = typeof(ProductExampleBuilder))]
        Product Post(Product resource);

        [Url("product/{id}")]
        [ProxyOperationDescription("Creates a new product")]
        [ProxyStatusCode(HttpStatusCode.OK, "Product updated")]
        [ProxyResourceExample(RequestBuilderType = typeof(ProductExampleBuilder), ResponseBuilderType = typeof(ProductExampleBuilder))]
        Product Put([ParameterConstraint(@"\d+"), ProxyRouteParameter(1)] int id, Product resource);

        [Url("product/{id}/in-stock/{inStock}")]
        [ProxyStatusCode(HttpStatusCode.OK, "Product updated")]
        [ProxyResourceExample(ResponseBuilderType = typeof(ProductExampleBuilder))]
        Product PatchStockStatus([ParameterConstraint(@"\d+"), ProxyRouteParameter(1)] int id,
                                 [ProxyRouteParameter("true")] bool? inStock);

        [Url("product/{id}")]
        [ProxyStatusCode(HttpStatusCode.NoContent, "Product deleted")]
        StatusResult DeleteById([ParameterConstraint(@"\d+"), ProxyRouteParameter(1)] int id);
    }
}
