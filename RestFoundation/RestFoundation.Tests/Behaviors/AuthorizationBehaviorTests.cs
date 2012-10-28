using System.Net;
using System.Security.Principal;
using NUnit.Framework;
using RestFoundation.Behaviors;
using RestFoundation.Runtime.Handlers;
using RestFoundation.Tests.Implementation.ServiceContracts;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.Behaviors
{
    [TestFixture]
    public class AuthorizationBehaviorTests
    {
        private const string UserName = "User";
        private const string UserRole = "Users";

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
        public void RequestWithSecurityPrincipalShouldNotThrow()
        {
            m_context.User = new GenericPrincipal(new GenericIdentity(UserName), new[] { "Administrators", UserRole });

            ISecureServiceBehavior behavior = new AuthorizationBehavior(UserRole);
            behavior.OnMethodAuthorizing(m_context, null);
        }

        [Test]
        public void RequestWithoutSecurityPrincipalShouldThrow()
        {
            ISecureServiceBehavior behavior = new AuthorizationBehavior(UserRole);

            m_context.User = null;

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
        public void RequestWithoutTheAuthorizedRoleShouldThrow()
        {
            m_context.User = new GenericPrincipal(new GenericIdentity(UserName), new[] { "Administrators", UserRole });

            ISecureServiceBehavior behavior = new AuthorizationBehavior("Managers", "Accountants");

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
