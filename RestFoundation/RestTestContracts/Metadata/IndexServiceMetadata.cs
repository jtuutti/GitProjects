using System;
using System.Net;
using RestFoundation.ServiceProxy;
using RestTestContracts.Resources;

namespace RestTestContracts.Metadata
{
    public class IndexServiceMetadata : ProxyMetadata<IIndexService>
    {
        public override void Initialize()
        {
            SetAuthentication(AuthenticationType.Digest, "admin", "~/async");

            ForMethod(x => x.Feed(null)).SetHidden();
            ForMethod(x => x.FileDownload()).SetHidden();
            ForMethod(x => x.FileUpload(null)).SetHidden();

            ForMethod(x => x.GetAll()).SetDescription("Gets all resources of type 'Person'")
                                      .SetResponseResourceExample(CreatePersonArrayExample());

            ForMethod(x => x.Get(null, null)).SetDescription("Gets resources of type 'Person' by ID")
                                             .SetRouteParameter("id", 1);

            ForMethod(x => x.Post(null)).SetDescription("Creates a new resource of type 'Person'")
                                        .SetResponseStatus(HttpStatusCode.Created, "Resource is created")
                                        .SetRequestResourceExample(CreatePersonRequestExample())
                                        .SetResponseResourceExample(CreatePersonResponseExample());

            ForMethod(x => x.Put(null, null)).SetDescription("Updates an existing resource of type 'Person'")
                                             .SetRouteParameter("id", 1)
                                             .SetResponseStatus(HttpStatusCode.OK, "Resource is updated")
                                             .SetRequestResourceExample(CreatePersonRequestExample())
                                             .SetResponseResourceExample(CreatePersonResponseExample());

            ForMethod(x => x.Patch(null, null)).SetDescription("Partially modifies an existing resource of type 'Person'")
                                               .SetRouteParameter("id", 1)
                                               .SetResponseStatus(HttpStatusCode.OK, "Resource is partially modified")
                                               .SetRequestResourceExample(CreatePersonRequestExample())
                                               .SetResponseResourceExample(CreatePersonResponseExample());

            ForMethod(x => x.Delete(null)).SetDescription("Deletes an existing resource of type 'Person' by name")
                                          .SetRouteParameter("name", "John Doe")
                                          .SetResponseStatus(HttpStatusCode.NoContent, "Resource is deleted");
        }

        private static Person[] CreatePersonArrayExample()
        {
            return new[]
            {
                new Person
                {
                    Name = "Joe Bloe",
                    Age = 41,
                    Values = new[] { "01/21/1971" }
                },
                new Person
                {
                    Name = "Mike Star",
                    Age = 21,
                    Values = new[] { "10/02/1991" }
                },
                new Person
                {
                    Name = "Beth Sue",
                    Age = 33
                }
            };
        }
        private static Person CreatePersonRequestExample()
        {
            return new Person
            {
                Name = "Joe Bloe",
                Age = 41
            };
        }

        private static Person CreatePersonResponseExample()
        {
            return new Person
            {
                Name = "Joe Bloe",
                Age = 41,
                Values = new[] { DateTime.Now.ToLongDateString() }
            };
        }
    }
}
