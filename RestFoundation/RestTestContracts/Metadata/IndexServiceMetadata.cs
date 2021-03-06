﻿using System;
using System.Net;
using RestFoundation.ServiceProxy;
using RestTestContracts.Resources;

namespace RestTestContracts.Metadata
{
    public class IndexServiceMetadata : ProxyMetadata<IIndexService>
    {
        private const string GetAllLongDescription = @"This service method can return a large number of person records.
                                                        
Use with caution!!!";

        public override void Initialize()
        {
            SetAuthentication(AuthenticationType.Digest, "admin", "~/secure");

            ForMethod(x => x.RedirectToGet(Arg<int>())).SetHidden();
            ForMethod(x => x.GetAllByFormat("json")).SetHidden();

            ForMethod(x => x.GetAll()).SetDescription("Gets all resources of type 'Person'")
                                      .SetLongDescription(GetAllLongDescription)
                                      .SetResponseResourceExample(CreatePersonArrayExample());

            ForMethod(x => x.GetAllAsync()).SetDescription("Gets all resources of type 'Person' asynchronously")
                                           .SetResponseResourceExample(CreatePersonArrayExample());

            ForMethod(x => x.GetAllChunked()).SetDescription("Gets all resources of type 'Person' using HTTP chunking capabilities")
                                             .SetResponseResourceExample(CreatePersonArrayExample());

            ForMethod(x => x.Get(1, Arg<string>())).SetDescription("Gets resources of type 'Person' by ID");

            ForMethod(x => x.Post(Arg<Person>())).SetDescription("Creates a new resource of type 'Person'")
                                                 .SetResponseStatus(HttpStatusCode.Created, "Resource is created")
                                                 .SetRequestResourceExample(CreatePersonRequestExample())
                                                 .SetResponseResourceExample(CreatePersonResponseExample());

            ForMethod(x => x.Put(1, Arg<Person>())).SetDescription("Updates an existing resource of type 'Person'")
                                                   .SetResponseStatus(HttpStatusCode.OK, "Resource is updated")
                                                   .SetRequestResourceExample(CreatePersonRequestExample())
                                                   .SetResponseResourceExample(CreatePersonResponseExample());

            ForMethod(x => x.Patch(1, Arg<Person>())).SetDescription("Partially modifies an existing resource of type 'Person'")
                                                     .SetResponseStatus(HttpStatusCode.OK, "Resource is partially modified")
                                                     .SetRequestResourceExample(CreatePersonRequestExample())
                                                     .SetResponseResourceExample(CreatePersonResponseExample());

            ForMethod(x => x.Delete("John Doe")).SetResponseStatus(HttpStatusCode.NoContent, "Resource is deleted");

            ForMethod(x => x.PostMultipleParameters(Arg<string>(), 0, Arg<DateTime?>())).SetDescription("Creates a new resource of type 'Person' from the body name-value attributes.")
                                                                                        .SetHeader("Content-Type", "application/x-www-form-urlencoded; charset=utf-8")
                                                                                        .SetRequestResourceExample("name=John+Doe&age=30&timestamp=2012-12-31+22:15:00");
        }

        private static Person[] CreatePersonArrayExample()
        {
            return new[]
            {
                new Person
                {
                    Name = "Joe Bloe",
                    Age = 41,
                    Values = new[] { "1971-01-21" },
                    TimeStamp = DateTime.Now
                },
                new Person
                {
                    Name = "Mike Star",
                    Age = 21,
                    Values = new[] { "1991-02-10" },
                    TimeStamp = DateTime.Now.AddMonths(-1)
                },
                new Person
                {
                    Name = "Beth Sue",
                    Age = 33,
                    TimeStamp = DateTime.Now.AddMonths(-2)
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
                Values = new[] { DateTime.Now.ToString("yyyy-MM-dd") },
                TimeStamp = DateTime.Now
            };
        }
    }
}
