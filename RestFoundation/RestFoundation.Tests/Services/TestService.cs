using System;
using System.Collections.Generic;
using System.Net;
using RestFoundation.Results;
using RestFoundation.Tests.ServiceContracts;

namespace RestFoundation.Tests.Services
{
    public class TestService : ITestService
    {
        public IServiceContext Context { get; set; }

        public IResult Get(int? id)
        {
            ValidateId(id);

            return Result.Ok;
        }

        public IResult GetAll(string orderBy)
        {
            if (String.IsNullOrWhiteSpace(orderBy))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "No order provided");
            }

            return Result.Ok;
        }

        public IResult Post()
        {
            return Result.ResponseStatus(HttpStatusCode.Created, "Created", new Dictionary<string, string>
                                                                            {
                                                                                { "Location", Context.Request.Url.OperationUrl + "/1" }
                                                                            });
        }

        public IResult Put(int? id)
        {
            ValidateId(id);

            return Result.NoContent;
        }

        public IResult Patch(int? id)
        {
            ValidateId(id);

            return Result.NoContent;
        }

        public void Delete(int? id)
        {
            ValidateId(id);
        }

        private static void ValidateId(int? id)
        {
            if (id == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "No ID provided");
            }

            if (id.Value < 0)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid ID provided");
            }
        }
    }
}
