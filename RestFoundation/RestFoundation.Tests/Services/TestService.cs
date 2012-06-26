using System.Collections.Generic;
using System.Net;
using RestFoundation.Tests.ServiceContracts;

namespace RestFoundation.Tests.Services
{
    public class TestService : ITestService
    {
        public IServiceContext Context { get; set; }
        public IHttpRequest Request { get; set; }
        public IHttpResponse Response { get; set; }

        public IResult Get(int? id)
        {
            return Result.Ok;
        }

        public IResult GetAll(string orderBy)
        {
            return Result.Ok;
        }

        public IResult Post()
        {
            return Result.SetStatus(HttpStatusCode.Created, "Created", new Dictionary<string, string>
                                                                       {
                                                                           { "Location", Request.Url.ServiceUrl + "/1" }
                                                                       });
        }

        public IResult Put(int? id)
        {
            return Result.NoContent;
        }

        public IResult Patch(int? id)
        {
            return Result.NoContent;
        }

        public IResult Delete(int? id)
        {
            return Result.NoContent;
        }
    }
}
