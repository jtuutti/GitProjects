using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using RestFoundation.Formatters;
using RestFoundation.Results;
using RestFoundation.Runtime.Handlers;
using RestFoundation.Tests.Implementation.Models;
using RestFoundation.Tests.Implementation.ServiceContracts;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.Formatters
{
    [TestFixture]
    public class XmlFormatterTests
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
        public void FormatRequest()
        {
            Model model = CreateModel();

            WriteBodyAsXml(model);

            var formatter = new XmlFormatter();
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

            var formatter = new XmlFormatter();
            var result = formatter.FormatResponse(m_context, typeof(Model), model) as XmlResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Content, Is.SameAs(model));

            result.Execute(m_context);

            string response = ReadResponseAsXml();
            Assert.That(response, Is.Not.Null);

            string serializedModel = SerializeModel(model);
            Assert.That(response, Is.EqualTo(serializedModel));
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

        private static string SerializeModel(Model model)
        {
            var memoryStream = new MemoryStream();

            var xmlWriter = new XmlTextWriter(new StreamWriter(memoryStream, Encoding.UTF8))
            {
                Formatting = Formatting.None
            };

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            var serializer = new XmlSerializer(model.GetType());
            serializer.Serialize(xmlWriter, model, namespaces);

            memoryStream.Seek(0, SeekOrigin.Begin);

            using (var reader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        private void WriteBodyAsXml(Model model)
        {
            var xmlWriter = new XmlTextWriter(new StreamWriter(m_context.Request.Body, Encoding.UTF8))
            {
                Formatting = Formatting.None
            };

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            var serializer = new XmlSerializer(model.GetType());
            serializer.Serialize(xmlWriter, model, namespaces);
        }

        private string ReadResponseAsXml()
        {
            m_context.Response.Output.Stream.Seek(0, SeekOrigin.Begin);

            var reader = new StreamReader(m_context.Response.Output.Stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }
    }
}
