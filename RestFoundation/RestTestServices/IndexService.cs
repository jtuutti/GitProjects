﻿using System;
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
            Response.Output.WriteFormat("GET : {0}", Request.QueryBag.X_http_method_override);

            return Result.Content("<br/><br/>GET completed", false);
        }

        public Person Post(Person resource)
        {
            if (resource == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid person data provided");
            }

            resource.Values = new[] { "New person added" };

            return resource;
        }

        public Person Put(int? id, Person resource)
        {
            if (resource == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid person data provided");
            }

            resource.Values = new[] { String.Format("Person #{0} updated", id) };

            return resource;
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

        public ContentResult Delete()
        {
            return Result.Content("Deleted");
        }
    }
}
