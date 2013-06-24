using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using RestFoundation.Formatters;
using RestFoundation.Results;
using RestFoundation.Tests.Implementation.Models;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.Formatters
{
    [TestFixture]
    public class JsonPContentTests
    {
        private const string CallbackFunction = "callback";

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

            var formatter = new JsonPFormatter();
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

            var formatter = new JsonPFormatter();
            var result = formatter.FormatResponse(m_context, typeof(Model), model, null) as JsonPResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Content, Is.SameAs(model));

            result.Callback = CallbackFunction;
            Assert.That(result.Callback, Is.EqualTo(CallbackFunction));

            result.Execute(m_context);

            string response = ReadResponseAsJsonP();
            Assert.That(response, Is.Not.Null);

            string serializedModel = SerializeModel(model, true);
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

        private static string SerializeModel(Model model, bool jsonP)
        {
            var serializedModel = JsonConvert.SerializeObject(model, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            if (!jsonP)
            {
                return serializedModel;
            }

            return String.Format("{0}({1});", CallbackFunction, serializedModel);
        }

        private void WriteBodyAsJson(Model model)
        {
            string jsonString = SerializeModel(model, false);

            var writer = new StreamWriter(m_context.Request.Body, Encoding.UTF8);
            writer.Write(jsonString);
            writer.Flush();
        }

        private string ReadResponseAsJsonP()
        {
            m_context.Response.Output.Stream.Seek(0, SeekOrigin.Begin);

            var reader = new StreamReader(m_context.Response.Output.Stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }
    }
}
