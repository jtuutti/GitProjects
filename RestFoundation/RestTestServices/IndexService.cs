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

        public ContentResult Get(int? id, string someGarbage)
        {
            Response.WriteFormat("GET : {0}", Request.QueryBag.X_http_method_override);
            Response.WriteLine().WriteFormat("Logging enabled: {0}", Request.HttpItems.TryGet("logging_enabled") ?? "false");

            return Result.Content("<br/><br/>Action completed", false);
        }

        public Person Post(Person resource)
        {
            if (resource == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid person data provided");
            }

            resource.Values = new[] { "Added" };

            return resource;
        }

        public StatusResult Put()
        {
            Response.Write("PUT");

            return Result.SetStatus(HttpStatusCode.OK, "Updated");
        }

        public RedirectResult Delete()
        {
            return Result.Redirect("http://www.google.com");
        }
    }
}
