using System.IO;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using RestFoundation.Formatters;
using RestFoundation.Results;
using RestFoundation.Tests.Implementation.Models;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.Formatters
{
    [TestFixture]
    public class JsonFormatterTests
    {
        private IServiceContext m_context;

        [SetUp]
        public void Initialize()
        {
            m_context = MockContextManager.GenerateContext();
        }

        [TearDown]
        public void ShutDown()
        {
            MockContextManager.DestroyContext();
        }

        [Test]
        public void FormatRequest()
        {
            Model model = CreateModel();

            WriteBodyAsJson(model);

            var formatter = new JsonFormatter();
            var resource = formatter.FormatRequest(m_context, typeof(Model)) as Model;

            Assert.That(resource, Is.Not.Null);
            Assert.That(resource.Id, Is.EqualTo(model.Id));
            Assert.That(resource.Name, Is.EqualTo(model.Name));
            Assert.That(resource.Items, Is.Not.Null);
            CollectionAssert.AreEqual(model.Items, resource.Items);
        }

        [Test]
        public void FormatResponse()
        {
            Model model = CreateModel();

            var formatter = new JsonFormatter();
            var result = formatter.FormatResponse(m_context, typeof(Model), model, null) as JsonResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Content, Is.SameAs(model));

            result.Execute(m_context);

            string response = ReadResponseAsJson();
            Assert.That(response, Is.Not.Null);

            string serializedModel = SerializeModel(model);
            Assert.That(response, Is.EqualTo(serializedModel));
        }

        [Test]
        public void FormatResponseWithPreferredMediaType()
        {
            Model model = CreateModel();

            var formatter = new JsonFormatter();
            var result = formatter.FormatResponse(m_context, typeof(Model), model, "application/json") as JsonResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Content, Is.SameAs(model));

            result.Execute(m_context);

            string response = ReadResponseAsJson();
            Assert.That(response, Is.Not.Null);

            string serializedModel = SerializeModel(model);
            Assert.That(response, Is.EqualTo(serializedModel));
        }

        private static Model CreateModel()
        {
            var model = new Model
            {
                Id = 1,
                Name = "John Doe",
                Items = new[] { "A", "B", "C" }
            };

            return model;
        }

        private static string SerializeModel(Model model)
        {
            return JsonConvert.SerializeObject(model);
        }

        private void WriteBodyAsJson(Model model)
        {
            string jsonString = SerializeModel(model);

            var writer = new StreamWriter(m_context.Request.Body, Encoding.UTF8);
            writer.Write(jsonString);
            writer.Flush();
        }

        private string ReadResponseAsJson()
        {
            m_context.Response.Output.Stream.Seek(0, SeekOrigin.Begin);

            var reader = new StreamReader(m_context.Response.Output.Stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }
    }
}
