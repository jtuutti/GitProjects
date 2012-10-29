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
        private readonly IHttpResponse m_response;
        private readonly ProductRepository m_repository;

        public SampleService(IHttpResponse response, ProductRepository repository)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            m_response = response;
            m_repository = repository;
        }

        public Product GetById(int id)
        {
            Product product = m_repository.GetAll().FirstOrDefault(p => p.ID == id);

            if (product == null)
            {
                m_response.SetStatus(HttpStatusCode.NotFound, "Product not found");
            }

            return product;
        }

        public IQueryable<Product> GetAll()
        {
            return m_repository.GetAll();
        }

        public Product Post(Product resource)
        {
            try
            {
                m_repository.Add(resource);
                m_response.SetStatus(HttpStatusCode.Created, "Product added");
            }
            catch (ArgumentException ex)
            {
                m_response.SetStatus(HttpStatusCode.BadRequest, ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                m_response.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
                return null;
            }

            return m_repository.GetAll().FirstOrDefault(p => p.ID == resource.ID);
        }

        public Product Put(int id, Product resource)
        {
            try
            {
                m_repository.Update(resource);
                m_response.SetStatus(HttpStatusCode.OK, "Product updated");
            }
            catch (ArgumentException ex)
            {
                m_response.SetStatus(HttpStatusCode.BadRequest, ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                m_response.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
                return null;
            }

            return m_repository.GetAll().FirstOrDefault(p => p.ID == resource.ID);
        }

        public Product PatchStockStatus(int id, bool? inStock)
        {
            if (!inStock.HasValue)
            {
                m_response.SetStatus(HttpStatusCode.BadRequest, "No InStock value was provided");
                return null;
            }

            try
            {
                m_repository.UpdateInStockStatus(id, inStock.Value);
                m_response.SetStatus(HttpStatusCode.OK, "Product updated");
            }
            catch (ArgumentException ex)
            {
                m_response.SetStatus(HttpStatusCode.BadRequest, ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                m_response.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
                return null;
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
            catch (ArgumentException ex)
            {
                return Result.ResponseStatus(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return Result.ResponseStatus(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}
