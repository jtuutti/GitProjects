using System.Net;
using System.Reflection;
using NUnit.Framework;
using RestFoundation.Behaviors;
using RestFoundation.Runtime.Handlers;
using RestFoundation.Tests.Implementation.Models;
using RestFoundation.Tests.Implementation.ServiceContracts;
using RestFoundation.Tests.Implementation.Services;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.Behaviors
{
    [TestFixture]
    public class ResourceValidationTests
    {
        private MockHandlerFactory m_factory;
        private IServiceContext m_context;
        private object m_service;
        private MethodInfo m_method;

        [SetUp]
        public void Initialize()
        {
            m_factory = new MockHandlerFactory();

            IRestHandler handler = m_factory.Create<ITestService>("~/test-service/new", m => m.Post(null));
            Assert.That(handler, Is.Not.Null);
            Assert.That(handler.Context, Is.Not.Null);
            
            m_context = handler.Context;
            m_service = new TestService();
            m_method = m_service.GetType().GetMethod("Post");
        }

        [TearDown]
        public void ShutDown()
        {
            m_factory.Dispose();
        }

        [Test]
        public void ValidResourceShouldNotThrow()
        {
            IServiceBehavior behavior = new ResourceValidationBehavior();

            var resource = new Model
            {
                ID = 1,
                Name = "Joe Bloe"
            };

            behavior.OnMethodExecuting(m_context, new MethodExecutingContext(m_service, m_method, resource));
        }

        [Test]
        public void EmptyResourceShouldThrow()
        {
            IServiceBehavior behavior = new ResourceValidationBehavior();

            var resource = new Model();

            try
            {
                behavior.OnMethodExecuting(m_context, new MethodExecutingContext(m_service, m_method, resource));
                Assert.Fail();
            }
            catch (HttpResponseException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            }
        }

        [Test]
        public void ResourceWithoutIDShouldThrow()
        {
            IServiceBehavior behavior = new ResourceValidationBehavior();

            var resource = new Model
            {
                Name = "Joe Bloe"
            };

            try
            {
                behavior.OnMethodExecuting(m_context, new MethodExecutingContext(m_service, m_method, resource));
                Assert.Fail();
            }
            catch (HttpResponseException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            }
        }

        [Test]
        public void ResourceWithEmptyNameShouldThrow()
        {
            IServiceBehavior behavior = new ResourceValidationBehavior();

            var resource = new Model
            {
                ID = 1,
                Name = ""
            };

            try
            {
                behavior.OnMethodExecuting(m_context, new MethodExecutingContext(m_service, m_method, resource));
                Assert.Fail();
            }
            catch (HttpResponseException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            }
        }

        [Test]
        public void ResourceWithNameOver25CharactersShouldThrow()
        {
            IServiceBehavior behavior = new ResourceValidationBehavior();

            var resource = new Model
            {
                ID = 1,
                Name = "Abcdefghijklmnopqrstuvwxyz"
            };

            try
            {
                behavior.OnMethodExecuting(m_context, new MethodExecutingContext(m_service, m_method, resource));
                Assert.Fail();
            }
            catch (HttpResponseException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            }
        }

        [Test]
        public void ResourceWithNameOf25CharactersShouldNotThrow()
        {
            IServiceBehavior behavior = new ResourceValidationBehavior();

            var resource = new Model
            {
                ID = 1,
                Name = "Abcdefghijklmnopqrstuvwxy"
            };

            behavior.OnMethodExecuting(m_context, new MethodExecutingContext(m_service, m_method, resource));
        }
    }
}
