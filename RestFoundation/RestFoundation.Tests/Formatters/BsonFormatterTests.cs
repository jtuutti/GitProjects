using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using RestFoundation.Client;
using RestFoundation.Formatters;
using RestFoundation.Results;
using RestFoundation.Runtime.Handlers;
using RestFoundation.Tests.Implementation.Models;
using RestFoundation.Tests.Implementation.ServiceContracts;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.Formatters
{
    [TestFixture]
    public class BsonFormatterTests
    {
        private MockHandlerFactory m_factory;
        private IServiceContext m_context;

        [SetUp]
        public void Initialize()
        {
            m_factory = new MockHandlerFactory();

            IRestHandler handler = m_factory.Create<ITestService>("~/test-service/new", m => m.Post(null));
            Assert.That(handler, Is.Not.Null);
            Assert.That(handler.Context, Is.Not.Null);

            m_context = handler.Context;
        }

        [TearDown]
        public void ShutDown()
        {
            m_factory.Dispose();
        }


        [Test]
        public void RandomTest()
        {
            IRestClient client = RestClientFactory.Create();
            IList<HttpMethod> methods = client.Options(new Uri("http://theory-m.com"));
        }

        [Test]
        public void FormatRequest()
        {
            Model model = CreateModel();

            WriteBodyAsBson(model);

            var formatter = new BsonFormatter();
            var resource = formatter.FormatRequest(m_context, typeof(Model)) as Model;

            Assert.That(resource, Is.Not.Null);
            Assert.That(resource.ID, Is.EqualTo(model.ID));
            Assert.That(resource.Name, Is.EqualTo(model.Name));
            Assert.That(resource.Items, Is.Not.Null);
            CollectionAssert.AreEqual(model.Items, resource.Items);
        }

        [Test]
        public void FormatResponse()
        {
            Model model = CreateModel();

            var formatter = new BsonFormatter();
            var result = formatter.FormatResponse(m_context, typeof(Model), model) as BsonResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Content, Is.SameAs(model));

            result.Execute(m_context);

            byte[] response = ReadResponseAsBson();
            Assert.That(response, Is.Not.Null);

            byte[] serializedModel = SerializeModel(model);
            CollectionAssert.AreEqual(response, serializedModel);
        }

        private static Model CreateModel()
        {
            var model = new Model
            {
                ID = 1,
                Name = "John Doe",
                Items = new[] { "A", "B", "C" }
            };

            return model;
        }

        public static byte[] SerializeModel(Model model)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new JsonSerializer();
                var bsonWriter = new BsonWriter(stream);
                serializer.Serialize(bsonWriter, model);
                bsonWriter.Flush();

                return stream.ToArray();
            }
        }

        private void WriteBodyAsBson(Model model)
        {
            byte[] bsonData = SerializeModel(model);

            m_context.Request.Body.Write(bsonData, 0, bsonData.Length);
            m_context.Request.Body.Flush();
        }

        private byte[] ReadResponseAsBson()
        {
            m_context.Response.Output.Stream.Seek(0, SeekOrigin.Begin);

            using (var reader = new StreamReader(m_context.Response.Output.Stream, Encoding.UTF8))
            {
                return Encoding.UTF8.GetBytes(reader.ReadToEnd()); // a way to ignore UTF-8 lead bytes
            }
        }
    }
}
