using System.Net;
using NUnit.Framework;
using RestFoundation.Behaviors;
using RestFoundation.Runtime.Handlers;
using RestFoundation.Tests.Implementation.ServiceContracts;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.Behaviors
{
    [TestFixture]
    public class AjaxOnlyBehaviorTests
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
        public void RequestWith_XRequestedWithHeader_ShouldNotThrow()
        {
            m_context.GetHttpContext().Request.Headers["X-Requested-With"] = "XmlHttpRequest";

            ISecureServiceBehavior behavior = new AjaxOnlyBehavior();
            behavior.OnMethodAuthorizing(m_context, null);
        }

        [Test]
        public void RequestWithout_XRequestedWithHeader_ShouldThrow404()
        {
            m_context.GetHttpContext().Request.Headers["X-Requested-With"] = null;

            ISecureServiceBehavior behavior = new AjaxOnlyBehavior();

            try
            {
                behavior.OnMethodAuthorizing(m_context, null);
                Assert.Fail();
            }
            catch (HttpResponseException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            }
        }
    }
}
