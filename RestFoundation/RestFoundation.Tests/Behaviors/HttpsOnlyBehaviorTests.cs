using System.Collections.Specialized;
using System.Net;
using NUnit.Framework;
using RestFoundation.Behaviors;
using RestFoundation.Runtime;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.Behaviors
{
    [TestFixture]
    public class HttpsOnlyBehaviorTests
    {
        [Test]
        public void SecureServiceRequestShouldNotThrow403()
        {
            var context = MockContextManager.GenerateContext(MockContextManager.RootVirtualPath, HttpMethod.Get, new NameValueCollection
            {
                { "X-Forwarded-Proto", "https" }
            });

            try
            {
                ISecureServiceBehavior behavior = new HttpsOnlyBehavior(true);

                behavior.OnMethodAuthorizing(context, null);
            }
            finally
            {
                MockContextManager.DestroyContext();
            }
        }

        [Test]
        public void UnsecureServiceRequestShouldThrow403()
        {
            var context = MockContextManager.GenerateContext();

            try
            {
                ISecureServiceBehavior behavior = new HttpsOnlyBehavior();

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
    }
}
