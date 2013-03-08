using System;
using System.Net;
using RestFoundation;
using RestFoundation.Runtime;
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
                Id = Convert.ToInt32(Request.QueryString.TryGet("id") ?? "0"),
                resource.Name,
                resource.Age
            };

            return result;
        }
    }
}
