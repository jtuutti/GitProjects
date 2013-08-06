using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using RestFoundation;
using RestFoundation.Client;
using RestFoundation.Results;
using RestFoundation.Runtime;
using RestTestContracts;
using RestTestContracts.Resources;
using RestTestServices.Repositories;

namespace RestTestServices
{
    public class IndexService : IIndexService
    {
        private readonly PersonRepository m_repository;

        public IndexService(PersonRepository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            m_repository = repository;
        }

        public IServiceContext Context { get; set; }

        public RedirectResult RedirectToGet(int id)
        {
            return Result.RedirectToAction<IIndexService>(c => c.Get(id > 0 ? id : 1, String.Empty));
        }

        public dynamic GetDynamicDict()
        {
            dynamic result = new DynamicResult();
            result.Name = new DynamicResult();
            result.Name.First = "Mike";
            result.Name.Last = "Hunt";
            result.Dat2 = new DateTime(2012, 12, 12);
            result.Url = new Uri("http://google.com?ab=c");
            return result;
        }

        public IQueryable<Person> GetAll()
        {
            var people = m_repository.GetAll();

            return new List<Person>(people).AsQueryable();
        }

        public async Task<IQueryable<Person>> GetAllAsync()
        {
            var client = RestClientFactory.Create();    
            var allowedMethods = await client.OptionsAsync(Context.Request.Url);

            if (!allowedMethods.Contains(HttpMethod.Get))
            {
                throw new HttpResponseException(HttpStatusCode.MethodNotAllowed, "GET is not allowed");
            }

            var people = m_repository.GetAll();

            var peopleQuery = await Task.Run(() => new List<Person>(people).AsQueryable(), Context.Response.GetCancellationToken());

            // validates that the async task returns to the right thread
            Context.Response.SetStatus(HttpStatusCode.OK, "Task completed asynchronously");

            return peopleQuery;
        }

        public IEnumerable<Person> GetAllChunked()
        {
            var people = m_repository.GetAll();

            return new List<Person>(people);
        }

        public IResult GetAllByFormat(string format)
        {
            var people = m_repository.GetAll();

            return Result.JsonOrXml(new List<Person>(people).AsQueryable(), format);
        }

        public ContentResult Get(int? id, string dummy)
        {
            if (!id.HasValue)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "No person ID provided");
            }

            Context.Response.Output.WriteFormat("GET : {0}", id);

            if (!String.IsNullOrEmpty(dummy))
            {
                Context.Response.Output.WriteFormat("<br/><br/>Dummy variable: {0}", dummy);
            }

            return Result.Content("<br/><br/>GET completed", false, "text/html");
        }

        public object Post(Person resource)
        {
            resource.Values = new[] { "New person added" };
            resource.TimeStamp = DateTime.Now;

            Context.Response.SetHeader("Location", Context.Request.Url.ToAbsoluteUrl("~/home/index/999"));

            return Result.ObjectWithResponseStatus(resource, HttpStatusCode.Created, "Person #999 created");
        }

        public Person Put(int? id, Person personToUpdate)
        {
            if (!id.HasValue)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "No person ID provided");
            }

            personToUpdate.Values = new[] { String.Format("Person #{0} updated", id) };

            return personToUpdate;
        }

        public Person Patch(int? id, Person resource)
        {
            if (!id.HasValue)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "No person ID provided");
            }

            resource.Values = new[] { String.Format("Person #{0} partially updated", id) };

            return resource;
        }

        public StatusCodeResult Delete(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid person's name provided");
            }

            return Result.ResponseStatus(HttpStatusCode.NoContent, String.Format("Person '{0}' deleted", name));
        }

        public Person PostMultipleParameters(string name, int age, DateTime? timestamp)
        {          
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid person's name provided");
            }

            return new Person
            {
                Name = name,
                Age = age,
                TimeStamp = timestamp.HasValue ? timestamp.Value : DateTime.MinValue
            };
        }
    }
}
