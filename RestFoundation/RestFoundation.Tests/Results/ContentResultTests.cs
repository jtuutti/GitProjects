using NUnit.Framework;
using RestFoundation.Results;
using RestFoundation.Runtime.Handlers;
using RestFoundation.Tests.Implementation.ServiceContracts;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.Results
{
    [TestFixture]
    public class ContentResultTests : ResultTestBase
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
        public void ContentResultWithPlainText()
        {
            const string ContentValue = "Hello world";
            const string ContentType = "text/plain";

            ContentResult result = Result.Content(ContentValue, true, ContentType);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ClearOutput, Is.True);
            Assert.That(result.Content, Is.EqualTo(ContentValue));
            Assert.That(result.ContentType, Is.EqualTo(ContentType));

            result.Execute(Context);
            Assert.That(GetResponseOutput(), Is.EqualTo(ContentValue));
        }

        [Test]
        public void ContentResultWithHtml()
        {
            const string ContentValue = "<div style='padding-top: 10px;'>Page executed</div>";
            const string ContentType = "text/html";

            ContentResult result = Result.Content(ContentValue, false, ContentType);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ClearOutput, Is.False);
            Assert.That(result.Content, Is.EqualTo(ContentValue));
            Assert.That(result.ContentType, Is.EqualTo(ContentType));

            result.Execute(Context);
            Assert.That(GetResponseOutput(), Is.EqualTo(ContentValue));
        }
    }
}
