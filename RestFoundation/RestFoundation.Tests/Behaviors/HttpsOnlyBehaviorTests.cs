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
        public void UnsecureServiceRequestShouldThrow403()
        {
            ISecureServiceBehavior behavior = new HttpsOnlyBehavior();

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
