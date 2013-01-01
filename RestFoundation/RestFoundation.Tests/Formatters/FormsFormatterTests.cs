using System.IO;
using System.Text;
using System.Web;
using NUnit.Framework;
using RestFoundation.Formatters;
using RestFoundation.Runtime.Handlers;
using RestFoundation.Tests.Implementation.Models;
using RestFoundation.Tests.Implementation.ServiceContracts;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.Formatters
{
    public class FormsFormatterTests
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

            WriteBodyAsFormsEncodedString(model);

            var formatter = new FormsFormatter();
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

            var formatter = new FormsFormatter();

            // "application/x-www-form-urlencoded" be used as an accepted media type
            Assert.Throws(typeof(HttpResponseException), () => formatter.FormatResponse(m_context, typeof(Model), model));
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

        private void WriteBodyAsFormsEncodedString(Model model)
        {
            var nameValueBuilder = new StringBuilder();
            nameValueBuilder.Append("ID=").Append(HttpUtility.UrlEncode(model.Id.ToString()));
            nameValueBuilder.Append("&Name=").Append(HttpUtility.UrlEncode(model.Name));

            foreach (string item in model.Items)
            {
                nameValueBuilder.Append("&Items=").Append(HttpUtility.UrlEncode(item));
            }

            var writer = new StreamWriter(m_context.Request.Body, Encoding.UTF8);
            writer.Write(nameValueBuilder.ToString());
            writer.Flush();
        }
    }
}
