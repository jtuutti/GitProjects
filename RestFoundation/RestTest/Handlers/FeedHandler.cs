using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml.Serialization;
using RestFoundation;
using RestFoundation.Results;
using RestFoundation.Runtime;
using RestTestContracts.Resources;
using RestTestServices.Repositories;

namespace RestTest.Handlers
{
    public class FeedHandler : HttpHandler
    {
        private readonly PersonRepository m_repository;

        public FeedHandler(PersonRepository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            m_repository = repository;
        }

        public override Task<IResult> ExecuteAsync(IServiceContext context)
        {
            FeedResult.SyndicationFormat feedFormat;

            var format = Convert.ToString(context.Request.RouteValues.TryGet("format"));

            if (format == null || !Enum.TryParse(format, true, out feedFormat))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType, "This feed only supports ATOM and RSS formats");
            }

            return TaskResult.Start(() =>
            {
                var people = m_repository.GetAll();

                var feedItems = new List<SyndicationItem>(people.Count);
                var xmlSerializer = new XmlSerializer(typeof(Person));

                for (int i = 0; i < people.Count; i++)
                {
                    var item = new SyndicationItem
                    {
                        Id = String.Format("urn:uuid:{0}", Guid.NewGuid().ToString().ToLowerInvariant()),
                        Title = new TextSyndicationContent("Person #" + (i + 1), TextSyndicationContentKind.Plaintext),
                        Content = new XmlSyndicationContent("application/vnd.person+xml", people[i], xmlSerializer),
                        PublishDate = DateTime.UtcNow,
                        LastUpdatedTime = DateTime.UtcNow
                    };

                    feedItems.Add(item);
                }

                var feed = new SyndicationFeed(feedItems)
                {
                    Id = "urn:uuid:6b46a53d-99c3-49e5-8828-c3d8aad2db0f",
                    Title = new TextSyndicationContent("People Feed", TextSyndicationContentKind.Plaintext),
                    Copyright = new TextSyndicationContent("(c) Rest Foundation, 2012", TextSyndicationContentKind.Plaintext),
                    Generator = "Rest Foundation Service"
                };

                return Result.Feed(feed, feedFormat);
            });
        }
    }
}