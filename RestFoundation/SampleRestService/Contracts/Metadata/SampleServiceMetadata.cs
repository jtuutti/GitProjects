using System;
using System.Linq;
using System.Net;
using RestFoundation.ServiceProxy;
using SampleRestService.DataAccess;
using SampleRestService.Resources;

namespace SampleRestService.Contracts.Metadata
{
    public class SampleServiceMetadata : ProxyMetadata<ISampleService>
    {
        private readonly ProductRepository m_repository;

        public SampleServiceMetadata(ProductRepository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            m_repository = repository;
        }

        public override void Initialize()
        {
            ForMethod(x => x.GetById(1)).SetResponseResourceExample(GetProduct());

            ForMethod(x => x.GetAll()).SetResponseResourceExample(GetProductArray());

            ForMethod(x => x.PostAsync(Arg<Product>())).SetRequestResourceExample(CreateNewProduct())
                                                       .SetResponseResourceExample(GetProduct())
                                                       .SetResponseStatus(HttpStatusCode.Created, "Product created");

            ForMethod(x => x.PutAsync(1, Arg<Product>())).SetRequestResourceExample(GetProduct())
                                                         .SetResponseResourceExample(GetProduct())
                                                         .SetResponseStatus(HttpStatusCode.OK, "Product updated");

            ForMethod(x => x.PatchStockStatus(1, true)).SetResponseResourceExample(GetProduct())
                                                       .SetResponseStatus(HttpStatusCode.OK, "Product updated");

            ForMethod(x => x.DeleteById(1)).SetResponseStatus(HttpStatusCode.NoContent, "Product deleted");
        }

        private static Product CreateNewProduct()
        {
            return new Product
            {
                Name = "Bananas",
                Price = 4.59m,
                InStock = true
            };
        }

        private Product GetProduct()
        {
            return m_repository.GetAll().FirstOrDefault();
        }

        private Product[] GetProductArray()
        {
            return m_repository.GetAll().Take(3).ToArray();
        }
    }
}