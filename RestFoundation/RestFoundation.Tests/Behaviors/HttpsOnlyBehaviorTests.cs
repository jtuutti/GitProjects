using System.Net;
using NUnit.Framework;
using RestFoundation.Behaviors;
using RestFoundation.Runtime.Handlers;
using RestFoundation.Tests.Implementation.ServiceContracts;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.Behaviors
{
    [TestFixture]
    public class HttpsOnlyBehaviorTests
    {
        private MockHandlerFactory m_factory;
        private IServiceContext m_context;

        [SetUp]
        public void Initialize()
        {
            m_factory = new MockHandlerFactory();

            IRestHandler handler = m_factory.Create<ITestService>("~/test-service/new", m => m.Post());
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
        public void UnsecureServiceRequestShouldThrow403()
        {
            ISecureServiceBehavior behavior = new HttpsOnlyBehavior();

            try
            {
                behavior.OnMethodAuthorizing(m_context, null, null);
                Assert.Fail();
            }
            catch (HttpResponseException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            }
        }
    }
}
