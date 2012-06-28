using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RestFoundation;
using RestFoundation.Results;
using RestTestContracts;
using RestTestContracts.Resources;

namespace RestTestServices
{
    public class IndexService : IIndexService
    {
        public IHttpRequest Request { get; set; }
        public IHttpResponse Response { get; set; }

        private readonly List<Person> people = new List<Person>
        {
            new Person { Name = "John", Age = 51, Values = new[] { "Manager", "old" } },
            new Person { Name = "Mike", Age = 16, Values = new string[0] },
            new Person { Name = "Beth", Age = 32, Values = new[] { "Secretary" } },
            new Person { Name = "Saul", Age = 62 }
        };

        public IQueryable<Person> GetAll()
        {
            return new List<Person>(people).AsQueryable();
        }

        public ContentResult Get(int? id, string someGarbage)
        {
            Response.Output.WriteFormat("GET : {0}", id);

            return Result.Content("<br/><br/>GET completed", false);
        }

        public Person Post(Person resource)
        {
            if (resource == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid person data provided");
            }

            resource.Values = new[] { "New person added" };

            Response.SetStatus(HttpStatusCode.Created);

            return resource;
        }

        public Person Put(int? id, Person personToUpdate)
        {
            if (personToUpdate == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid person data provided");
            }

            personToUpdate.Values = new[] { String.Format("Person #{0} updated", id) };

            return personToUpdate;
        }

        public Person Patch(int? id, Person resource)
        {
            if (resource == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid person data provided");
            }

            resource.Values = new[] { String.Format("Person #{0} partially updated", id) };

            return resource;
        }

        public StatusResult Delete(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid person's name provided");
            }

            return Result.NoContent;
        }
    }
}
