using System;
using System.Net;
using RestFoundation;
using RestFoundation.Runtime;
using RestTestContracts;

namespace RestTestServices
{
    public class DynamicService : IDynamicService
    {
        public dynamic Post(dynamic resource, int? id, IHttpRequest request)
        {
            if (resource == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "No resource provided");
            }

            dynamic result = new
            {
                Id = Convert.ToInt32(id.HasValue ? id.Value : 0),
                resource.Name,
                resource.Age
            };

            return result;
        }
    }
}
