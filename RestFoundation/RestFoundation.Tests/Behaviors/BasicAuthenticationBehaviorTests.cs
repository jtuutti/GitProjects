using System;
using System.Net;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using RestFoundation.Behaviors;
using RestFoundation.Runtime.Handlers;
using RestFoundation.Tests.Implementation.Authorization;
using RestFoundation.Tests.Implementation.ServiceContracts;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.Behaviors
{
    [TestFixture]
    public class BasicAuthenticationBehaviorTests
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
        public void RequestWithoutAuthorizationHeaderShouldThrow()
        {
            ISecureServiceBehavior behavior = new BasicAuthenticationBehavior();

            m_context.GetHttpContext().Request.Headers["Authorization"] = null;

            try
            {
                behavior.OnMethodAuthorizing(m_context, null);
                Assert.Fail();
            }
            catch (HttpResponseException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            }
        }

        [Test]
        public void RequestWithBasicAuthorizationHeaderShouldNotThrow()
        {
            ISecureServiceBehavior behavior = new BasicAuthenticationBehavior(new TestAuthorizationManager());

            m_context.GetHttpContext().Request.Headers["Authorization"] = String.Format("Basic {0}", ToBase64String("user1:test123"));

            behavior.OnMethodAuthorizing(m_context, null);

            Assert.That(m_context.User, Is.Not.Null);
            Assert.That(m_context.IsAuthenticated, Is.True);
            Assert.That(m_context.User, Is.InstanceOf<GenericPrincipal>());
            Assert.That(m_context.User.Identity.Name, Is.EqualTo("user1"));
            Assert.That(m_context.User.IsInRole("Testers"), Is.True);
        }

        [Test]
        public void RequestWithDigestAuthorizationHeaderShouldThrow()
        {
            ISecureServiceBehavior behavior = new BasicAuthenticationBehavior();

            m_context.GetHttpContext().Request.Headers["Authorization"] = "Digest username=\"user1\", realm=\"http://localhost\", nonce=\"dcd98b7102dd2f0e8b11d0f600bfb0c093\", qop=auth, cnonce=\"0a4f113b\"";

            try
            {
                behavior.OnMethodAuthorizing(m_context, null);
                Assert.Fail();
            }
            catch (HttpResponseException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            }
        }

        [Test]
        public void RequestWithBasicAuthorizationHeaderAndWrongCredentialsShouldThrow()
        {
            ISecureServiceBehavior behavior = new BasicAuthenticationBehavior();

            m_context.GetHttpContext().Request.Headers["Authorization"] = String.Format("Basic {0}", ToBase64String("user1:wrong-password"));

            try
            {
                behavior.OnMethodAuthorizing(m_context, null);
                Assert.Fail();
            }
            catch (HttpResponseException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            }
        }

        private static string ToBase64String(string credentials)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
        }
    }
}
