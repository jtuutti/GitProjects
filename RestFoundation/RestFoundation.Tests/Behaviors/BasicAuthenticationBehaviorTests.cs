using System;
using System.Collections.Specialized;
using System.Net;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using RestFoundation.Behaviors;
using RestFoundation.Runtime;
using RestFoundation.Tests.Implementation.Authorization;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.Behaviors
{
    [TestFixture]
    public class BasicAuthenticationBehaviorTests
    {
        private const string ServiceUri = "/test-service/new";

        [Test]
        public void RequestWithoutAuthorizationHeaderShouldThrow()
        {
            ISecureServiceBehavior behavior = new BasicAuthenticationBehavior();
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
        public void RequestWithBasicAuthorizationHeaderShouldNotThrow()
        {
            string authorizationHeader = String.Concat("Basic ", ToBase64String("user1:test123"));

            ISecureServiceBehavior behavior = new BasicAuthenticationBehavior(new TestAuthorizationManager());
            IServiceContext context = GenerateAuthorizedContext(authorizationHeader);

            try
            {
                behavior.OnMethodAuthorizing(context, null);

                Assert.That(context.User, Is.Not.Null);
                Assert.That(context.IsAuthenticated, Is.True);
                Assert.That(context.User, Is.InstanceOf<GenericPrincipal>());
                Assert.That(context.User.Identity.Name, Is.EqualTo("user1"));
                Assert.That(context.User.IsInRole("Testers"), Is.True);
            }
            finally
            {
                MockContextManager.DestroyContext();
            }
        }

        [Test]
        public void RequestWithDigestAuthorizationHeaderShouldThrow()
        {
            const string authorizationHeader = "Digest username=\"user1\", realm=\"http://localhost\", nonce=\"dcd98b7102dd2f0e8b11d0f600bfb0c093\", qop=auth, cnonce=\"0a4f113b\"";

            ISecureServiceBehavior behavior = new BasicAuthenticationBehavior(new TestAuthorizationManager());
            IServiceContext context = GenerateAuthorizedContext(authorizationHeader);

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
        public void RequestWithBasicAuthorizationHeaderAndWrongCredentialsShouldThrow()
        {
            string authorizationHeader = String.Format("Basic {0}", ToBase64String("user1:wrong-password"));

            ISecureServiceBehavior behavior = new BasicAuthenticationBehavior(new TestAuthorizationManager());
            IServiceContext context = GenerateAuthorizedContext(authorizationHeader);

            try
            {
                behavior.OnMethodAuthorizing(context, null);
                Assert.Fail();
            }
            catch (HttpResponseException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
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

        private static IServiceContext GenerateAuthorizedContext(string authorizationHeader)
        {
            var headers = new NameValueCollection
            {
                { "Authorization", authorizationHeader }
            };

            return MockContextManager.GenerateContext(ServiceUri, HttpMethod.Post, headers);
        }

        private static string ToBase64String(string credentials)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
        }
    }
}
