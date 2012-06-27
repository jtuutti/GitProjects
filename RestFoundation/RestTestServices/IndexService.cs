using System;
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

        public IResult Get(int? id, string someGarbage)
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

        public ContentResult Delete(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid person's name provided");
            }

            return Result.ContentFormat("Person '{0}' deleted", name);
        }
    }
}
