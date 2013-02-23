using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using RestFoundation;
using RestFoundation.Results;
using SampleRestService.Contracts;
using SampleRestService.DataAccess;
using SampleRestService.Resources;

namespace SampleRestService.Services
{
    public class SampleService : ISampleService
    {
        private readonly IServiceContext m_context;
        private readonly ProductRepository m_repository;

        public SampleService(IServiceContext context, ProductRepository repository)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            m_context = context;
            m_repository = repository;
        }

        public Product GetById(int id)
        {
            Product product = m_repository.GetAll().FirstOrDefault(p => p.ID == id);

            if (product == null)
            {
                m_context.Response.SetStatus(HttpStatusCode.NotFound, "Product not found");
            }

            return product;
        }

        public IQueryable<Product> GetAll()
        {
            return m_repository.GetAll();
        }

        public async Task<Product> PostAsync(Product resource)
        {
            // resource validation is happening in the ValidateResourceAttribute defined on the service contract

            try
            {
                await m_repository.AddAsync(resource);
            }
            catch (ArgumentException ex) // invalid input arguments passed to the repository
            {
                throw new HttpResourceFaultException(new[] { ex.Message });
            }

            Product addedResource = m_repository.GetAll().FirstOrDefault(p => p.ID == resource.ID);

            var responseHeaders = new Dictionary<string, string>
            {
                { // Location header with a fully qualified GET URL pointing to the resource by its ID
                    m_context.Response.HeaderNames.Location,
                    m_context.GetPath<ISampleService>(null, m => m.GetById(resource.ID), null, UriSegments.CreateFromHttpRequest(m_context.Request))
                }
            };

            return Result.ObjectWithResponseStatus(addedResource, HttpStatusCode.Created, "Product added", responseHeaders);
        }

        public async Task<Product> PutAsync(int id, Product resource)
        {
            // resource validation is happening in the ValidateResourceAttribute defined on the service contract

            try
            {
                resource.ID = id;
                await m_repository.UpdateAsync(resource);
            }
            catch (ArgumentException ex) // invalid input arguments passed to the repository
            {
                throw new HttpResourceFaultException(new[] { ex.Message });
            }

            Product updatedResource = m_repository.GetAll().FirstOrDefault(p => p.ID == resource.ID);

            return Result.ObjectWithResponseStatus(updatedResource, HttpStatusCode.OK, "Product updated");
        }

        public Product PatchStockStatus(int id, bool inStock)
        {
            try
            {
                m_repository.UpdateInStockStatus(id, inStock);
            }
            catch (ArgumentException ex) // invalid input arguments passed to the repository
            {
                throw new HttpResourceFaultException(new[] { ex.Message });
            }

            Product updatedResource = m_repository.GetAll().FirstOrDefault(p => p.ID == id);

            return Result.ObjectWithResponseStatus(updatedResource, HttpStatusCode.OK, "Product updated");
        }

        public StatusResult DeleteById(int id)
        {
            try
            {
                // We do not care about the return value because the HTTP DELETE method must behave the same if executed multiple times
                m_repository.Delete(id);

                return Result.ResponseStatus(HttpStatusCode.NoContent, "Product deleted");
            }
            catch (ArgumentException ex) // invalid input arguments passed to the repository
            {
                return Result.ResponseStatus(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}
