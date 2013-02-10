using System.Net;
using NUnit.Framework;
using RestFoundation.Behaviors;
using RestFoundation.Runtime.Handlers;
using RestFoundation.Tests.Implementation.ServiceContracts;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.Behaviors
{
    [TestFixture]
    public class AclBehaviorTests
    {
        private MockHandlerFactory m_factory;
        private IServiceContext m_context;

        [SetUp]
        public void Initialize()
        {
            m_factory = new MockHandlerFactory();

            IRestServiceHandler handler = m_factory.Create<ITestService>("~/test-service/new", m => m.Post(null));
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
        public void RequestWithLocalAclSectionShouldNotThrow()
        {
            ISecureServiceBehavior behavior = new AclBehavior("localAcl");
            behavior.OnMethodAuthorizing(m_context, null);
        }

        [Test]
        public void RequestWithRemoteAclSectionShouldThrow()
        {
            ISecureServiceBehavior behavior = new AclBehavior("remoteAcl");

            try
            {
                behavior.OnMethodAuthorizing(m_context, null);
                Assert.Fail();
            }
            catch (HttpResponseException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            }
        }

        [Test]
        public void RequestWithNonExistingAclSectionShouldThrow()
        {
            ISecureServiceBehavior behavior = new AclBehavior("invalidAcl");

            try
            {
                behavior.OnMethodAuthorizing(m_context, null);
                Assert.Fail();
            }
            catch (HttpResponseException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            }
        }
    }
}
