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
    /// <summary>
    /// A sample service.
    /// </summary>
    [ProxyMetadata(typeof(SampleServiceMetadata))]
    public interface ISampleService
    {
        /// <summary>
        /// Gets a product by id.
        /// </summary>
        /// <param name="id">The product id</param>
        /// <returns>The product with the provided id or null</returns>
        [Url("product/{id}")]
        Product GetById([Constraint(Constraint.UnsignedInteger)] int id);

        /// <summary>
        /// Gets all products.
        /// </summary>
        /// <returns>The collection of products</returns>
        [Url("products")]
        IQueryable<Product> GetAll();

        /// <summary>
        /// Adds a new product.
        /// </summary>
        /// <param name="resource">The product</param>
        /// <returns>The product after it has been added</returns>
        [Url("product"), AssertValidation(true)]
        Task<Product> PostAsync(Product resource);

        /// <summary>
        /// Updates a product.
        /// </summary>
        /// <param name="id">The product id</param>
        /// <param name="resource">The product</param>
        /// <returns>The product after it has been updated</returns>
        [Url("product/{id}"), AssertValidation(true)]
        Task<Product> PutAsync([Constraint(Constraint.UnsignedInteger)] int id, Product resource);

        /// <summary>
        /// Updates stock status for a product with the provided id.
        /// </summary>
        /// <param name="id">The product id</param>
        /// <param name="inStock">True if the product is in stock; otherwise false</param>
        /// <returns>The product after it has been updated</returns>
        Product PatchStockStatus([Constraint(Constraint.UnsignedInteger)] int id, bool inStock = true);

        /// <summary>
        /// Deletes a product by id.
        /// </summary>
        /// <param name="id">The product id</param>
        /// <returns>The HTTP status code</returns>
        [Url("product/{id}")]
        StatusCodeResult DeleteById([Constraint(Constraint.UnsignedInteger)] int id);
    }
}
