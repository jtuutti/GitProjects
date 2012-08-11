using System.Net;
using NUnit.Framework;
using RestFoundation.Behaviors;
using RestFoundation.Runtime.Handlers;
using RestFoundation.Tests.Implementation.Models;
using RestFoundation.Tests.Implementation.ServiceContracts;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.Behaviors
{
    [TestFixture]
    public class ResourceValidationTests
    {
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
        public void ValidResourceShouldNotThrow()
        {
            IServiceBehavior behavior = new ResourceValidationBehavior();

            var resource = new Model
            {
                ID = 1,
                Name = "Joe Bloe"
            };

            behavior.OnMethodExecuting(m_context, null, null, resource);
        }

        [Test]
        public void EmptyResourceShouldThrow()
        {
            IServiceBehavior behavior = new ResourceValidationBehavior();

            var resource = new Model();

            try
            {
                behavior.OnMethodExecuting(m_context, null, null, resource);
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
                behavior.OnMethodExecuting(m_context, null, null, resource);
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
                behavior.OnMethodExecuting(m_context, null, null, resource);
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
                behavior.OnMethodExecuting(m_context, null, null, resource);
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

            behavior.OnMethodExecuting(m_context, null, null, resource);
        }
    }
}
