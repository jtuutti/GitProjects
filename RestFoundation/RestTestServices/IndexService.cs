using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Xml.Serialization;
using RestFoundation;
using RestFoundation.Results;
using RestTestContracts;
using RestTestContracts.Resources;

namespace RestTestServices
{
    public class IndexService : IIndexService
    {
        public IServiceContext Context { get; set; }

        private readonly List<Person> people = new List<Person>
        {
            new Person { Name = "John", Age = 51, Values = new[] { "Manager", "old" } },
            new Person { Name = "Mike", Age = 16, Values = new string[0] },
            new Person { Name = "Beth", Age = 32, Values = new[] { "Secretary" } },
            new Person { Name = "Saul", Age = 62 }
        };

        public FeedResult Feed(string format)
        {
            FeedResult.SyndicationFormat feedFormat;

            if (!Enum.TryParse(format, true, out feedFormat))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType, "The feed only supports ATOM and RSS formats");
            }

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
        }

        public FileResultBase FileDownload()
        {
            return new FilePathResult
            {
                FilePath = @"D:\american_pie_5.avi",
                ContentType = "video/x-ms-wmv",
                ContentDisposition = "attachment; filename=movie.avi"
            };
        }

        public ContentResult FileUpload(ICollection<IUploadedFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return Result.Content("No files");
            }

            return Result.Content("Files: " + String.Join(", ", files.Select(f => f.Name)));
        }

        public IQueryable<Person> GetAll()
        {
            return new List<Person>(people).AsQueryable();
        }

        public ContentResult Get(int? id, string someGarbage)
        {
            Context.Response.Output.WriteFormat("GET : {0}", id);

            return Result.Content("<br/><br/>GET completed", false);
        }

        public IResult Post(Person resource)
        {
            if (resource == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid person data provided");
            }

            resource.Values = new[] { "New person added" };

            var responseHeaders = new Dictionary<string, string>
            {
                { "Location", Context.Request.Url.ToAbsoluteUrl("~/home/index/999") }
            };

            return Result.Resource(resource, HttpStatusCode.Created, "Person #999 created", responseHeaders);
        }

        public Person Put(int? id, Person personToUpdate)
        {
            if (personToUpdate == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid person data provided");
            }

            personToUpdate.Values = new[] { String.Format("Person #{0} updated", id) };

            return personToUpdate;
        }

        public Person Patch(int? id, Person resource)
        {
            if (resource == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid person data provided");
            }

            resource.Values = new[] { String.Format("Person #{0} partially updated", id) };

            return resource;
        }

        public StatusResult Delete(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid person's name provided");
            }

            return Result.ResponseStatus(HttpStatusCode.NoContent, String.Format("Person '{0}' deleted", name));
        }
    }
}
