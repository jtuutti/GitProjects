using System.Net;
using NUnit.Framework;
using RestFoundation.Behaviors;
using RestFoundation.Runtime;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.Behaviors
{
    [TestFixture]
    public class AclBehaviorTests
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
