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

            IRestServiceHandler handler = m_factory.Create<ITestService>("~/test-service/new", m => m.Post(null));
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
        public void ValidResourceShouldNotCreateValidationErrors()
        {
            IServiceBehavior behavior = new ValidationBehavior();

            var resource = new Model
            {
                Id = 1,
                Name = "Joe Bloe"
            };

            behavior.OnMethodExecuting(m_context, new MethodExecutingContext(m_service, m_method, resource));

            Assert.That(m_context.Request.ResourceState.IsValid, Is.True);
            Assert.That(m_context.Request.ResourceState.Count, Is.EqualTo(0));
        }

        [Test]
        public void EmptyResourceShouldCreateValidationErrors()
        {
            IServiceBehavior behavior = new ValidationBehavior();

            var resource = new Model();

            behavior.OnMethodExecuting(m_context, new MethodExecutingContext(m_service, m_method, resource));

            Assert.That(m_context.Request.ResourceState.IsValid, Is.False);
            Assert.That(m_context.Request.ResourceState.Count, Is.GreaterThan(0));
        }

        [Test]
        public void ResourceWithoutIDShouldCreateValidationErrors()
        {
            IServiceBehavior behavior = new ValidationBehavior();

            var resource = new Model
            {
                Name = "Joe Bloe"
            };

            behavior.OnMethodExecuting(m_context, new MethodExecutingContext(m_service, m_method, resource));

            Assert.That(m_context.Request.ResourceState.IsValid, Is.False);
            Assert.That(m_context.Request.ResourceState.Count, Is.GreaterThan(0));
        }

        [Test]
        public void ResourceWithEmptyNameShouldCreateValidationErrors()
        {
            IServiceBehavior behavior = new ValidationBehavior();

            var resource = new Model
            {
                Id = 1,
                Name = ""
            };

            behavior.OnMethodExecuting(m_context, new MethodExecutingContext(m_service, m_method, resource));

            Assert.That(m_context.Request.ResourceState.IsValid, Is.False);
            Assert.That(m_context.Request.ResourceState.Count, Is.GreaterThan(0));
        }

        [Test]
        public void ResourceWithNameOver25CharactersShouldCreateValidationErrors()
        {
            IServiceBehavior behavior = new ValidationBehavior();

            var resource = new Model
            {
                Id = 1,
                Name = "Abcdefghijklmnopqrstuvwxyz"
            };

            behavior.OnMethodExecuting(m_context, new MethodExecutingContext(m_service, m_method, resource));

            Assert.That(m_context.Request.ResourceState.IsValid, Is.False);
            Assert.That(m_context.Request.ResourceState.Count, Is.GreaterThan(0));
        }

        [Test]
        public void ResourceWithNameOf25CharactersShouldNotCreateValidationErrors()
        {
            IServiceBehavior behavior = new ValidationBehavior();

            var resource = new Model
            {
                Id = 1,
                Name = "Abcdefghijklmnopqrstuvwxy"
            };

            behavior.OnMethodExecuting(m_context, new MethodExecutingContext(m_service, m_method, resource));

            Assert.That(m_context.Request.ResourceState.IsValid, Is.True);
            Assert.That(m_context.Request.ResourceState.Count, Is.EqualTo(0));
        }
    }
}
