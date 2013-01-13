using System;
using System.Linq;
using System.Net;
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

        public Product Post(Product resource)
        {
            if (!m_context.Request.ResourceState.IsValid)
            {
                // Iterate through m_context.Request.ResourceState to handle or log errors

                throw new HttpResourceFaultException();
            }

            try
            {
                m_repository.Add(resource);

                // string relativeLocationUrl = m_context.GetPath<ISampleService>(m => m.GetById(resource.ID);

                string absoluteLocationUrl = m_context.GetPath<ISampleService>(null, m => m.GetById(resource.ID), null, UriSegments.CreateFromHttpRequest(m_context.Request));

                m_context.Response.SetHeader(m_context.Response.HeaderNames.Location, absoluteLocationUrl);
                m_context.Response.SetStatus(HttpStatusCode.Created, "Product added");
            }
            catch (ArgumentException ex) // invalid input arguments passed to the repository
            {
                throw new HttpResourceFaultException(new[] { ex.Message });
            }

            return m_repository.GetAll().FirstOrDefault(p => p.ID == resource.ID);
        }

        public Product Put(int id, Product resource)
        {
            if (!m_context.Request.ResourceState.IsValid)
            {
                // Iterate through m_context.Request.ResourceState to handle or log errors

                throw new HttpResourceFaultException();
            }

            try
            {
                m_repository.Update(resource);
                m_context.Response.SetStatus(HttpStatusCode.OK, "Product updated");
            }
            catch (ArgumentException ex) // invalid input arguments passed to the repository
            {
                throw new HttpResourceFaultException(new[] { ex.Message });
            }

            return m_repository.GetAll().FirstOrDefault(p => p.ID == resource.ID);
        }

        public Product PatchStockStatus(int id, bool inStock)
        {
            try
            {
                m_repository.UpdateInStockStatus(id, inStock);
                m_context.Response.SetStatus(HttpStatusCode.OK, "Product updated");
            }
            catch (ArgumentException ex) // invalid input arguments passed to the repository
            {
                throw new HttpResourceFaultException(new[] { ex.Message });
            }

            return m_repository.GetAll().FirstOrDefault(p => p.ID == id);
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
