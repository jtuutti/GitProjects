﻿using System.Net;
using RestFoundation;
using RestTestContracts;

namespace RestTestServices
{
    public class DynamicService : IDynamicService
    {
        public IHttpRequest Request { get; set; }

        public dynamic Post(dynamic resource)
        {
            if (resource == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "No resource provided");
            }

            dynamic result = new
            {
                Request.QueryBag.Id,
                resource.Name,
                resource.Age
            };

            return result;
        }
    }
}