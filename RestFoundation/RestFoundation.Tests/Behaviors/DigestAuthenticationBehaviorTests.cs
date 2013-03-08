using System;
using System.Net;
using NUnit.Framework;
using RestFoundation.Behaviors;
using RestFoundation.Runtime;
using RestFoundation.Runtime.Handlers;
using RestFoundation.Security;
using RestFoundation.Tests.Implementation.Authorization;
using RestFoundation.Tests.Implementation.ServiceContracts;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.Behaviors
{
    [TestFixture]
    public class DigestAuthenticationBehaviorTests
    {
        private const string UserName = "user1";
        private const string Password = "test123";
        private const string ClientNonce = "abcd1234";
        private const string NonceCount = "00000001";
        private const string ServiceUri = "/test-service/new";

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
            ISecureServiceBehavior behavior = new DigestAuthenticationBehavior();

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
        public void RequestUsingDigestWithAuthShouldNotThrow()
        {
            ISecureServiceBehavior behavior = new DigestAuthenticationBehavior(new TestAuthorizationManager());

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

            string authenticateHeaderString = m_context.Response.GetHeader("WWW-Authenticate");
            Assert.That(authenticateHeaderString, Is.Not.Null);
            Assert.That(authenticateHeaderString, Is.StringStarting("Digest"));

            string authorizationHeaderString = String.Format("Digest {0}, username=\"{1}\", cnonce=\"{2}\", nc=\"{3}\", uri=\"{4}\"",
                                                             authenticateHeaderString.Replace("Digest ", String.Empty),
                                                             UserName,
                                                             ClientNonce,
                                                             NonceCount,
                                                             ServiceUri);

            AuthorizationHeader authorizationHeader;
            Assert.That(AuthorizationHeaderParser.TryParse(authorizationHeaderString, out authorizationHeader));

            string response;

            using (var encoder = new MD5Encoder())
            {
                string ha1 = encoder.Encode(String.Format("{0}:{1}:{2}", UserName, authorizationHeader.Parameters.Get("realm"), Password));
                string ha2 = encoder.Encode(String.Format("{0}:{1}", "POST", ServiceUri));

                response = encoder.Encode(String.Format("{0}:{1}:{2}:{3}:{4}:{5}", ha1, authorizationHeader.Parameters.Get("nonce"), NonceCount, ClientNonce, "auth", ha2));
            }

            m_context.GetHttpContext().Request.Headers["Authorization"] = String.Format("{0}, response=\"{1}\"", authorizationHeaderString, response);

            behavior.OnMethodAuthorizing(m_context, null);
        }

        [Test]
        public void RequestUsingDigestWithoutAuthShouldNotThrow()
        {
            ISecureServiceBehavior behavior = new DigestAuthenticationBehavior(new TestAuthorizationManager())
            {
                Qop = DigestAuthenticationBehavior.QualityOfProtection.None
            };

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

            string authenticateHeaderString = m_context.Response.GetHeader("WWW-Authenticate");
            Assert.That(authenticateHeaderString, Is.Not.Null);
            Assert.That(authenticateHeaderString, Is.StringStarting("Digest"));

            string authorizationHeaderString = String.Format("Digest {0}, username=\"{1}\", uri=\"{2}\"",
                                                             authenticateHeaderString.Replace("Digest ", String.Empty),
                                                             UserName,
                                                             ServiceUri);

            AuthorizationHeader authorizationHeader;
            Assert.That(AuthorizationHeaderParser.TryParse(authorizationHeaderString, out authorizationHeader));

            string response;

            using (var encoder = new MD5Encoder())
            {
                string ha1 = encoder.Encode(String.Format("{0}:{1}:{2}", UserName, authorizationHeader.Parameters.Get("realm"), Password));
                string ha2 = encoder.Encode(String.Format("{0}:{1}", "POST", ServiceUri));

                response = encoder.Encode(String.Format("{0}:{1}:{2}", ha1, authorizationHeader.Parameters.Get("nonce"), ha2));
            }

            m_context.GetHttpContext().Request.Headers["Authorization"] = String.Format("{0}, response=\"{1}\"", authorizationHeaderString, response);

            behavior.OnMethodAuthorizing(m_context, null);
        }
    }
}
