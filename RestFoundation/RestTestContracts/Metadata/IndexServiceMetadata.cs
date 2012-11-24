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

            ForMethod(x => x.Get(1, null)).SetDescription("Gets resources of type 'Person' by ID");

            ForMethod(x => x.Post(null)).SetDescription("Creates a new resource of type 'Person'")
                                        .SetResponseStatus(HttpStatusCode.Created, "Resource is created")
                                        .SetRequestResourceExample(CreatePersonRequestExample())
                                        .SetResponseResourceExample(CreatePersonResponseExample());

            ForMethod(x => x.Put(1, null)).SetDescription("Updates an existing resource of type 'Person'")
                                          .SetResponseStatus(HttpStatusCode.OK, "Resource is updated")
                                          .SetRequestResourceExample(CreatePersonRequestExample())
                                          .SetResponseResourceExample(CreatePersonResponseExample());

            ForMethod(x => x.Patch(1, null)).SetDescription("Partially modifies an existing resource of type 'Person'")
                                            .SetResponseStatus(HttpStatusCode.OK, "Resource is partially modified")
                                            .SetRequestResourceExample(CreatePersonRequestExample())
                                            .SetResponseResourceExample(CreatePersonResponseExample());

            ForMethod(x => x.Delete("John Doe")).SetDescription("Deletes an existing resource of type 'Person' by name")
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
                    Values = new[] { "1971-01-21" }
                },
                new Person
                {
                    Name = "Mike Star",
                    Age = 21,
                    Values = new[] { "1991-02-10" }
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
                Values = new[] { DateTime.Now.ToString("yyyy-MM-dd") }
            };
        }
    }
}
