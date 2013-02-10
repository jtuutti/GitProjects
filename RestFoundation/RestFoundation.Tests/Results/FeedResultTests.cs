using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using RestFoundation.Results;
using RestFoundation.Runtime.Handlers;
using RestFoundation.Tests.Implementation.Models;
using RestFoundation.Tests.Implementation.ServiceContracts;
using RestFoundation.UnitTesting;
using SyndicationFormat = RestFoundation.Results.FeedResult.SyndicationFormat;

namespace RestFoundation.Tests.Results
{
    [TestFixture]
    public class FeedResultTests : ResultTestBase
    {
        private MockHandlerFactory m_factory;

        [SetUp]
        public void Initialize()
        {
            m_factory = new MockHandlerFactory();

            IRestServiceHandler handler = m_factory.Create<ITestService>("~/test-service/new", m => m.Post(null));
            Assert.That(handler, Is.Not.Null);
            Assert.That(handler.Context, Is.Not.Null);
            
            Context = handler.Context;
        }

        [TearDown]
        public void ShutDown()
        {
            m_factory.Dispose();
        }

        [Test]
        public void FeedResultUsingAtom()
        {
            const SyndicationFormat FeedFormat = SyndicationFormat.Atom;

            SyndicationFeed feed = CreateFeed();

            FeedResult result = Result.Feed(feed, FeedFormat);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Feed, Is.EqualTo(feed));
            Assert.That(result.Format, Is.EqualTo(FeedFormat));

            result.Execute(Context);

            string output = StripDatetime(GetResponseOutput());
            string feedValue = StripDatetime(SerializeFeed(feed, FeedFormat));

            Assert.That(output, Is.EqualTo(feedValue));
        }

        [Test]
        public void FeedResultUsingRss()
        {
            const SyndicationFormat FeedFormat = SyndicationFormat.Rss;

            SyndicationFeed feed = CreateFeed();

            FeedResult result = Result.Feed(feed, FeedFormat);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Feed, Is.EqualTo(feed));
            Assert.That(result.Format, Is.EqualTo(FeedFormat));

            result.Execute(Context);

            string output = StripDatetime(GetResponseOutput());
            string feedValue = StripDatetime(SerializeFeed(feed, FeedFormat));

            Assert.That(output, Is.EqualTo(feedValue));
        }

        public string SerializeFeed(SyndicationFeed feed, SyndicationFormat format)
        {
            using (var stream = new MemoryStream())
            {
                var writer = XmlWriter.Create(stream);

                if (format == SyndicationFormat.Atom)
                {
                    feed.GetAtom10Formatter().WriteTo(writer);
                }
                else
                {
                    feed.GetRss20Formatter().WriteTo(writer);
                }

                writer.Flush();

                stream.Position = 0;

                var reader = new StreamReader(stream, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }

        private static SyndicationFeed CreateFeed()
        {
            var resources = new[]
                {
                    new Model { Id = 1, Name = "Joe Bloe", Items = new[] { "Owner" } },
                    new Model { Id = 2, Name = "Jane Bloe", Items = new[] { "Secretary" } }
                };

            var feedItems = new List<SyndicationItem>(resources.Length);
            var xmlSerializer = new XmlSerializer(typeof(Model));

            for (int i = 0; i < resources.Length; i++)
            {
                var item = new SyndicationItem
                           {
                               Id = String.Format("urn:uuid:{0}", Guid.NewGuid().ToString().ToLowerInvariant()),
                               Title = new TextSyndicationContent("Resource #" + (i + 1), TextSyndicationContentKind.Plaintext),
                               Content = new XmlSyndicationContent("application/vnd.model+xml", resources[i], xmlSerializer),
                               PublishDate = DateTime.UtcNow,
                               LastUpdatedTime = DateTime.UtcNow
                           };

                feedItems.Add(item);
            }

            return new SyndicationFeed(feedItems)
            {
                 Id = "urn:uuid:" + Guid.NewGuid().ToString().ToLowerInvariant(),
                 Title = new TextSyndicationContent("Resource Feed", TextSyndicationContentKind.Plaintext),
                 Generator = "Feed Result Unit Tests"
            };
        }

        private static string StripDatetime(string output)
        {
            // Prevents responses that vary by 1-2 ms from failing the assert
            return Regex.Replace(output, @"\d\d\d\d-\d\d-\d\dT\d\d\:\d\d\:\d\dZ", "datetime");
        }
    }
}
