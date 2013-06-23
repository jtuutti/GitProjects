using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using NUnit.Framework;
using RestFoundation.Behaviors;
using RestFoundation.Runtime;
using RestFoundation.Security;
using RestFoundation.Tests.Implementation.Authorization;
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

        [Test]
        public void RequestWithoutAuthorizationHeaderShouldThrow()
        {
            ISecureServiceBehavior behavior = new DigestAuthenticationBehavior();
            IServiceContext context = GenerateInitialContext();

            try
            {
                behavior.OnMethodAuthorizing(context, null);
                Assert.Fail();
            }
            catch (HttpResponseException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            }
            finally
            {
                MockContextManager.DestroyContext();
            }
        }

        [Test]
        public void RequestUsingDigestWithAuthShouldNotThrow()
        {
            ISecureServiceBehavior behavior = new DigestAuthenticationBehavior(new TestAuthorizationManager());

            string authorizationHeaderString;

            try
            {
                // creating initial unauthorized context
                IServiceContext initialContext = GenerateInitialContext();

                try
                {
                    behavior.OnMethodAuthorizing(initialContext, null);
                    Assert.Fail();
                }
                catch (HttpResponseException ex)
                {
                    Assert.That(ex.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
                }

                // generating authorization header
                string authenticateHeaderString = initialContext.Response.GetHeader("WWW-Authenticate");
                Assert.That(authenticateHeaderString, Is.Not.Null);
                Assert.That(authenticateHeaderString, Is.StringStarting("Digest"));

                authorizationHeaderString = String.Format("Digest {0} username=\"{1}\", cnonce=\"{2}\", nc=\"{3}\", uri=\"{4}\"",
                                                                 authenticateHeaderString.Replace("Digest ", String.Empty),
                                                                 UserName,
                                                                 ClientNonce,
                                                                 NonceCount,
                                                                 ServiceUri);
            }
            finally
            {
                MockContextManager.DestroyContext();
            }

            AuthorizationHeader authorizationHeader;
            Assert.That(AuthorizationHeaderParser.TryParse(authorizationHeaderString, out authorizationHeader));

            // generating digest response
            string response;

            using (var encoder = new MD5Encoder())
            {
                string ha1 = encoder.Encode(String.Format("{0}:{1}:{2}", UserName, authorizationHeader.Parameters.Get("realm"), Password));
                string ha2 = encoder.Encode(String.Format("{0}:{1}", "POST", ServiceUri));

                response = encoder.Encode(String.Format("{0}:{1}:{2}:{3}:{4}:{5}", ha1, authorizationHeader.Parameters.Get("nonce"), NonceCount, ClientNonce, "auth", ha2));
            }

            try
            {
                // creating authorized context
                IServiceContext authorizedContext = GenerateAuthorizedContext(authorizationHeaderString, response);
                behavior.OnMethodAuthorizing(authorizedContext, null);
            }
            finally
            {
                MockContextManager.DestroyContext();
            }
        }

        [Test]
        public void RequestUsingDigestWithoutAuthShouldNotThrow()
        {
            ISecureServiceBehavior behavior = new DigestAuthenticationBehavior(new TestAuthorizationManager())
            {
                Qop = DigestAuthenticationBehavior.QualityOfProtection.None
            };

            string authorizationHeaderString;

            try
            {
                // creating initial unauthorized context
                IServiceContext initialContext = GenerateInitialContext();

                try
                {
                    behavior.OnMethodAuthorizing(initialContext, null);
                    Assert.Fail();
                }
                catch (HttpResponseException ex)
                {
                    Assert.That(ex.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
                }

                // generating authorization header
                string authenticateHeaderString = initialContext.Response.GetHeader("WWW-Authenticate");
                Assert.That(authenticateHeaderString, Is.Not.Null);
                Assert.That(authenticateHeaderString, Is.StringStarting("Digest"));

                authorizationHeaderString = String.Format("Digest {0} username=\"{1}\", uri=\"{2}\"",
                                                          authenticateHeaderString.Replace("Digest ", String.Empty),
                                                          UserName,
                                                          ServiceUri);
            }
            finally
            {
                MockContextManager.DestroyContext();
            }

            AuthorizationHeader authorizationHeader;
            Assert.That(AuthorizationHeaderParser.TryParse(authorizationHeaderString, out authorizationHeader));

            // generating digest response
            string response;

            using (var encoder = new MD5Encoder())
            {
                string ha1 = encoder.Encode(String.Format("{0}:{1}:{2}", UserName, authorizationHeader.Parameters.Get("realm"), Password));
                string ha2 = encoder.Encode(String.Format("{0}:{1}", "POST", ServiceUri));

                response = encoder.Encode(String.Format("{0}:{1}:{2}", ha1, authorizationHeader.Parameters.Get("nonce"), ha2));
            }

            try
            {
                // creating authorized context
                IServiceContext authorizedContext = GenerateAuthorizedContext(authorizationHeaderString, response);
                behavior.OnMethodAuthorizing(authorizedContext, null);
            }
            finally
            {
                MockContextManager.DestroyContext();
            }
        }

        private static IServiceContext GenerateInitialContext()
        {
            return MockContextManager.GenerateContext(ServiceUri, HttpMethod.Post);
        }

        private static IServiceContext GenerateAuthorizedContext(string authorizationHeader, string response)
        {
            var headers = new NameValueCollection
            {
                { "Authorization", String.Format(CultureInfo.InvariantCulture, "{0}, response=\"{1}\"", authorizationHeader, response) }
            };

            return MockContextManager.GenerateContext(ServiceUri, HttpMethod.Post, headers);
        }
    }
}
