using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using RestFoundation.Behaviors;
using RestFoundation.Results;
using RestFoundation.Runtime.Handlers;
using RestFoundation.Tests.Implementation.ServiceContracts;
using RestFoundation.Tests.Implementation.Services;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.Behaviors
{
    [TestFixture]
    public class BehaviorContextTests
    {
        private MockHandlerFactory m_factory;

        [SetUp]
        public void Initialize()
        {
            m_factory = new MockHandlerFactory();

            IRestHandler testHandler = m_factory.Create<ITestService>("~/test-service/new", m => m.Post(null));
            Assert.That(testHandler, Is.Not.Null);
            Assert.That(testHandler.Context, Is.Not.Null);

            IRestHandler selfContainedHandler = m_factory.Create<TestSelfContainedService>("~/test-ok-fail/ok", m => m.GetOK());
            Assert.That(selfContainedHandler, Is.Not.Null);
            Assert.That(selfContainedHandler.Context, Is.Not.Null);
        }

        [TearDown]
        public void ShutDown()
        {
            m_factory.Dispose();
        }

        [Test]
        public void MethodAuthorizingContextShouldBePopulatedForGetHttpMethod()
        {
            var service = new TestService();

            MethodInfo method = service.GetType().GetMethod("GetAll");
            Assert.That(method, Is.Not.Null);

            var behaviorContext = new MethodAuthorizingContext(service, method);
            Assert.That(behaviorContext.Service, Is.SameAs(service));
            Assert.That(behaviorContext.Method, Is.SameAs(method));
            Assert.That(behaviorContext.GetServiceContractType(), Is.EqualTo(typeof(ITestService)));
            Assert.That(behaviorContext.GetServiceType(), Is.EqualTo(typeof(TestService)));
            Assert.That(behaviorContext.GetMethodName(), Is.EqualTo(method.Name));

            var httpMethods = behaviorContext.GetSupportedHttpMethods().ToList();
            Assert.That(httpMethods.Contains(HttpMethod.Get), Is.True);
            Assert.That(httpMethods.Contains(HttpMethod.Head), Is.True);
            Assert.That(httpMethods.Contains(HttpMethod.Post), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Put), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Patch), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Delete), Is.False);
        }

        [Test]
        public void MethodAuthorizingContextShouldBePopulatedForPostHttpMethod()
        {
            var service = new TestService();

            MethodInfo method = service.GetType().GetMethod("Post");
            Assert.That(method, Is.Not.Null);

            var behaviorContext = new MethodAuthorizingContext(service, method);
            Assert.That(behaviorContext.Service, Is.SameAs(service));
            Assert.That(behaviorContext.Method, Is.SameAs(method));
            Assert.That(behaviorContext.GetServiceContractType(), Is.EqualTo(typeof(ITestService)));
            Assert.That(behaviorContext.GetServiceType(), Is.EqualTo(typeof(TestService)));
            Assert.That(behaviorContext.GetMethodName(), Is.EqualTo(method.Name));

            var httpMethods = behaviorContext.GetSupportedHttpMethods().ToList();
            Assert.That(httpMethods.Contains(HttpMethod.Get), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Head), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Post), Is.True);
            Assert.That(httpMethods.Contains(HttpMethod.Put), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Patch), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Delete), Is.False);
        }

        [Test]
        public void MethodAuthorizingContextShouldBePopulatedForPutHttpMethod()
        {
            var service = new TestService();

            MethodInfo method = service.GetType().GetMethod("Put");
            Assert.That(method, Is.Not.Null);

            var behaviorContext = new MethodAuthorizingContext(service, method);
            Assert.That(behaviorContext.Service, Is.SameAs(service));
            Assert.That(behaviorContext.Method, Is.SameAs(method));
            Assert.That(behaviorContext.GetServiceContractType(), Is.EqualTo(typeof(ITestService)));
            Assert.That(behaviorContext.GetServiceType(), Is.EqualTo(typeof(TestService)));
            Assert.That(behaviorContext.GetMethodName(), Is.EqualTo(method.Name));

            var httpMethods = behaviorContext.GetSupportedHttpMethods().ToList();
            Assert.That(httpMethods.Contains(HttpMethod.Get), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Head), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Post), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Put), Is.True);
            Assert.That(httpMethods.Contains(HttpMethod.Patch), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Delete), Is.False);
        }

        [Test]
        public void MethodAuthorizingContextShouldBePopulatedForPatchHttpMethod()
        {
            var service = new TestService();

            MethodInfo method = service.GetType().GetMethod("Patch");
            Assert.That(method, Is.Not.Null);

            var behaviorContext = new MethodAuthorizingContext(service, method);
            Assert.That(behaviorContext.Service, Is.SameAs(service));
            Assert.That(behaviorContext.Method, Is.SameAs(method));
            Assert.That(behaviorContext.GetServiceContractType(), Is.EqualTo(typeof(ITestService)));
            Assert.That(behaviorContext.GetServiceType(), Is.EqualTo(typeof(TestService)));
            Assert.That(behaviorContext.GetMethodName(), Is.EqualTo(method.Name));

            var httpMethods = behaviorContext.GetSupportedHttpMethods().ToList();
            Assert.That(httpMethods.Contains(HttpMethod.Get), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Head), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Post), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Put), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Patch), Is.True);
            Assert.That(httpMethods.Contains(HttpMethod.Delete), Is.False);
        }

        [Test]
        public void MethodAuthorizingContextShouldBePopulatedForDeleteHttpMethod()
        {
            var service = new TestService();

            MethodInfo method = service.GetType().GetMethod("Delete");
            Assert.That(method, Is.Not.Null);

            var behaviorContext = new MethodAuthorizingContext(service, method);
            Assert.That(behaviorContext.Service, Is.SameAs(service));
            Assert.That(behaviorContext.Method, Is.SameAs(method));
            Assert.That(behaviorContext.GetServiceContractType(), Is.EqualTo(typeof(ITestService)));
            Assert.That(behaviorContext.GetServiceType(), Is.EqualTo(typeof(TestService)));
            Assert.That(behaviorContext.GetMethodName(), Is.EqualTo(method.Name));

            var httpMethods = behaviorContext.GetSupportedHttpMethods().ToList();
            Assert.That(httpMethods.Contains(HttpMethod.Get), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Head), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Post), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Put), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Patch), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Delete), Is.True);
        }

        [Test]
        public void MethodExecutingContextShouldBePopulatedForGetHttpMethod()
        {
            var service = new TestService();

            MethodInfo method = service.GetType().GetMethod("GetAll");
            Assert.That(method, Is.Not.Null);

            var behaviorContext = new MethodExecutingContext(service, method, null);
            Assert.That(behaviorContext.Service, Is.SameAs(service));
            Assert.That(behaviorContext.Method, Is.SameAs(method));
            Assert.That(behaviorContext.Resource, Is.Null);
            Assert.That(behaviorContext.GetServiceContractType(), Is.EqualTo(typeof(ITestService)));
            Assert.That(behaviorContext.GetServiceType(), Is.EqualTo(typeof(TestService)));
            Assert.That(behaviorContext.GetMethodName(), Is.EqualTo(method.Name));

            var httpMethods = behaviorContext.GetSupportedHttpMethods().ToList();
            Assert.That(httpMethods.Contains(HttpMethod.Get), Is.True);
            Assert.That(httpMethods.Contains(HttpMethod.Head), Is.True);
            Assert.That(httpMethods.Contains(HttpMethod.Post), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Put), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Patch), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Delete), Is.False);
        }

        [Test]
        public void MethodExecutingContextShouldBePopulatedForPostHttpMethod()
        {
            var service = new TestService();

            MethodInfo method = service.GetType().GetMethod("Post");
            Assert.That(method, Is.Not.Null);

            var resource = new object();

            var behaviorContext = new MethodExecutingContext(service, method, resource);
            Assert.That(behaviorContext.Service, Is.SameAs(service));
            Assert.That(behaviorContext.Method, Is.SameAs(method));
            Assert.That(behaviorContext.Resource, Is.SameAs(resource));
            Assert.That(behaviorContext.GetServiceContractType(), Is.EqualTo(typeof(ITestService)));
            Assert.That(behaviorContext.GetServiceType(), Is.EqualTo(typeof(TestService)));
            Assert.That(behaviorContext.GetMethodName(), Is.EqualTo(method.Name));

            var httpMethods = behaviorContext.GetSupportedHttpMethods().ToList();
            Assert.That(httpMethods.Contains(HttpMethod.Get), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Head), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Post), Is.True);
            Assert.That(httpMethods.Contains(HttpMethod.Put), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Patch), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Delete), Is.False);
        }

        [Test]
        public void MethodExecutingContextShouldBePopulatedForPutHttpMethod()
        {
            var service = new TestService();

            MethodInfo method = service.GetType().GetMethod("Put");
            Assert.That(method, Is.Not.Null);

            var resource = new object();

            var behaviorContext = new MethodExecutingContext(service, method, resource);
            Assert.That(behaviorContext.Service, Is.SameAs(service));
            Assert.That(behaviorContext.Method, Is.SameAs(method));
            Assert.That(behaviorContext.Resource, Is.SameAs(resource));
            Assert.That(behaviorContext.GetServiceContractType(), Is.EqualTo(typeof(ITestService)));
            Assert.That(behaviorContext.GetServiceType(), Is.EqualTo(typeof(TestService)));
            Assert.That(behaviorContext.GetMethodName(), Is.EqualTo(method.Name));

            var httpMethods = behaviorContext.GetSupportedHttpMethods().ToList();
            Assert.That(httpMethods.Contains(HttpMethod.Get), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Head), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Post), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Put), Is.True);
            Assert.That(httpMethods.Contains(HttpMethod.Patch), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Delete), Is.False);
        }

        [Test]
        public void MethodExecutingContextShouldBePopulatedForPatchHttpMethod()
        {
            var service = new TestService();

            MethodInfo method = service.GetType().GetMethod("Patch");
            Assert.That(method, Is.Not.Null);

            var resource = new object();

            var behaviorContext = new MethodExecutingContext(service, method, resource);
            Assert.That(behaviorContext.Service, Is.SameAs(service));
            Assert.That(behaviorContext.Method, Is.SameAs(method));
            Assert.That(behaviorContext.Resource, Is.SameAs(resource));
            Assert.That(behaviorContext.GetServiceContractType(), Is.EqualTo(typeof(ITestService)));
            Assert.That(behaviorContext.GetServiceType(), Is.EqualTo(typeof(TestService)));
            Assert.That(behaviorContext.GetMethodName(), Is.EqualTo(method.Name));

            var httpMethods = behaviorContext.GetSupportedHttpMethods().ToList();
            Assert.That(httpMethods.Contains(HttpMethod.Get), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Head), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Post), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Put), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Patch), Is.True);
            Assert.That(httpMethods.Contains(HttpMethod.Delete), Is.False);
        }

        [Test]
        public void MethodExecutingContextShouldBePopulatedForDeleteHttpMethod()
        {
            var service = new TestService();

            MethodInfo method = service.GetType().GetMethod("Delete");
            Assert.That(method, Is.Not.Null);

            var behaviorContext = new MethodExecutingContext(service, method, null);
            Assert.That(behaviorContext.Service, Is.SameAs(service));
            Assert.That(behaviorContext.Method, Is.SameAs(method));
            Assert.That(behaviorContext.Resource, Is.Null);
            Assert.That(behaviorContext.GetServiceContractType(), Is.EqualTo(typeof(ITestService)));
            Assert.That(behaviorContext.GetServiceType(), Is.EqualTo(typeof(TestService)));
            Assert.That(behaviorContext.GetMethodName(), Is.EqualTo(method.Name));

            var httpMethods = behaviorContext.GetSupportedHttpMethods().ToList();
            Assert.That(httpMethods.Contains(HttpMethod.Get), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Head), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Post), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Put), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Patch), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Delete), Is.True);
        }

        [Test]
        public void MethodExecutedContextShouldBePopulatedForGetHttpMethod()
        {
            var service = new TestService();

            MethodInfo method = service.GetType().GetMethod("GetAll");
            Assert.That(method, Is.Not.Null);

            var returnedObject = new ContentResult
            {
                Content = "ReturnedData",
                ContentType = "text/plain"
            };

            var behaviorContext = new MethodExecutedContext(service, method, returnedObject);
            Assert.That(behaviorContext.Service, Is.SameAs(service));
            Assert.That(behaviorContext.Method, Is.SameAs(method));
            Assert.That(behaviorContext.ReturnedObject, Is.SameAs(returnedObject));
            Assert.That(behaviorContext.GetServiceContractType(), Is.EqualTo(typeof(ITestService)));
            Assert.That(behaviorContext.GetServiceType(), Is.EqualTo(typeof(TestService)));
            Assert.That(behaviorContext.GetMethodName(), Is.EqualTo(method.Name));

            var httpMethods = behaviorContext.GetSupportedHttpMethods().ToList();
            Assert.That(httpMethods.Contains(HttpMethod.Get), Is.True);
            Assert.That(httpMethods.Contains(HttpMethod.Head), Is.True);
            Assert.That(httpMethods.Contains(HttpMethod.Post), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Put), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Patch), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Delete), Is.False);
        }

        [Test]
        public void MethodExecutedContextShouldBePopulatedForPostHttpMethod()
        {
            var service = new TestService();

            MethodInfo method = service.GetType().GetMethod("Post");
            Assert.That(method, Is.Not.Null);

            var returnedObject = new ContentResult
            {
                Content = "ReturnedData",
                ContentType = "text/plain"
            };

            var behaviorContext = new MethodExecutedContext(service, method, returnedObject);
            Assert.That(behaviorContext.Service, Is.SameAs(service));
            Assert.That(behaviorContext.Method, Is.SameAs(method));
            Assert.That(behaviorContext.ReturnedObject, Is.SameAs(returnedObject));
            Assert.That(behaviorContext.GetServiceContractType(), Is.EqualTo(typeof(ITestService)));
            Assert.That(behaviorContext.GetServiceType(), Is.EqualTo(typeof(TestService)));
            Assert.That(behaviorContext.GetMethodName(), Is.EqualTo(method.Name));

            var httpMethods = behaviorContext.GetSupportedHttpMethods().ToList();
            Assert.That(httpMethods.Contains(HttpMethod.Get), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Head), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Post), Is.True);
            Assert.That(httpMethods.Contains(HttpMethod.Put), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Patch), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Delete), Is.False);
        }

        [Test]
        public void MethodExecutedContextShouldBePopulatedForPutHttpMethod()
        {
            var service = new TestService();

            MethodInfo method = service.GetType().GetMethod("Put");
            Assert.That(method, Is.Not.Null);

            var returnedObject = new ContentResult
            {
                Content = "ReturnedData",
                ContentType = "text/plain"
            };

            var behaviorContext = new MethodExecutedContext(service, method, returnedObject);
            Assert.That(behaviorContext.Service, Is.SameAs(service));
            Assert.That(behaviorContext.Method, Is.SameAs(method));
            Assert.That(behaviorContext.ReturnedObject, Is.SameAs(returnedObject));
            Assert.That(behaviorContext.GetServiceContractType(), Is.EqualTo(typeof(ITestService)));
            Assert.That(behaviorContext.GetServiceType(), Is.EqualTo(typeof(TestService)));
            Assert.That(behaviorContext.GetMethodName(), Is.EqualTo(method.Name));

            var httpMethods = behaviorContext.GetSupportedHttpMethods().ToList();
            Assert.That(httpMethods.Contains(HttpMethod.Get), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Head), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Post), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Put), Is.True);
            Assert.That(httpMethods.Contains(HttpMethod.Patch), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Delete), Is.False);
        }

        [Test]
        public void MethodExecutedContextShouldBePopulatedForPatchHttpMethod()
        {
            var service = new TestService();

            MethodInfo method = service.GetType().GetMethod("Patch");
            Assert.That(method, Is.Not.Null);

            var returnedObject = new ContentResult
            {
                Content = "ReturnedData",
                ContentType = "text/plain"
            };

            var behaviorContext = new MethodExecutedContext(service, method, returnedObject);
            Assert.That(behaviorContext.Service, Is.SameAs(service));
            Assert.That(behaviorContext.Method, Is.SameAs(method));
            Assert.That(behaviorContext.ReturnedObject, Is.SameAs(returnedObject));
            Assert.That(behaviorContext.GetServiceContractType(), Is.EqualTo(typeof(ITestService)));
            Assert.That(behaviorContext.GetServiceType(), Is.EqualTo(typeof(TestService)));
            Assert.That(behaviorContext.GetMethodName(), Is.EqualTo(method.Name));

            var httpMethods = behaviorContext.GetSupportedHttpMethods().ToList();
            Assert.That(httpMethods.Contains(HttpMethod.Get), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Head), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Post), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Put), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Patch), Is.True);
            Assert.That(httpMethods.Contains(HttpMethod.Delete), Is.False);
        }

        [Test]
        public void MethodExecutedContextShouldBePopulatedForDeleteHttpMethod()
        {
            var service = new TestService();

            MethodInfo method = service.GetType().GetMethod("Delete");
            Assert.That(method, Is.Not.Null);

            var behaviorContext = new MethodExecutedContext(service, method, null);
            Assert.That(behaviorContext.Service, Is.SameAs(service));
            Assert.That(behaviorContext.Method, Is.SameAs(method));
            Assert.That(behaviorContext.ReturnedObject, Is.Null);
            Assert.That(behaviorContext.GetServiceContractType(), Is.EqualTo(typeof(ITestService)));
            Assert.That(behaviorContext.GetServiceType(), Is.EqualTo(typeof(TestService)));
            Assert.That(behaviorContext.GetMethodName(), Is.EqualTo(method.Name));

            var httpMethods = behaviorContext.GetSupportedHttpMethods().ToList();
            Assert.That(httpMethods.Contains(HttpMethod.Get), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Head), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Post), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Put), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Patch), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Delete), Is.True);
        }

        [Test]
        public void MethodAuthorizingContextShouldBePopulatedForSelfContainedService()
        {
            var service = new TestSelfContainedService();

            MethodInfo method = service.GetType().GetMethod("GetOK");
            Assert.That(method, Is.Not.Null);

            var behaviorContext = new MethodAuthorizingContext(service, method);
            Assert.That(behaviorContext.Service, Is.SameAs(service));
            Assert.That(behaviorContext.Method, Is.SameAs(method));
            Assert.That(behaviorContext.GetServiceContractType(), Is.EqualTo(typeof(TestSelfContainedService)));
            Assert.That(behaviorContext.GetServiceType(), Is.EqualTo(typeof(TestSelfContainedService)));
            Assert.That(behaviorContext.GetMethodName(), Is.EqualTo(method.Name));

            var httpMethods = behaviorContext.GetSupportedHttpMethods().ToList();
            Assert.That(httpMethods.Contains(HttpMethod.Get), Is.True);
            Assert.That(httpMethods.Contains(HttpMethod.Head), Is.True);
            Assert.That(httpMethods.Contains(HttpMethod.Post), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Put), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Patch), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Delete), Is.False);
        }

        [Test]
        public void MethodExecutingContextShouldBePopulatedForSelfContainedService()
        {
            var service = new TestSelfContainedService();

            MethodInfo method = service.GetType().GetMethod("GetOK");
            Assert.That(method, Is.Not.Null);

            var behaviorContext = new MethodExecutingContext(service, method, null);
            Assert.That(behaviorContext.Service, Is.SameAs(service));
            Assert.That(behaviorContext.Method, Is.SameAs(method));
            Assert.That(behaviorContext.Resource, Is.Null);
            Assert.That(behaviorContext.GetServiceContractType(), Is.EqualTo(typeof(TestSelfContainedService)));
            Assert.That(behaviorContext.GetServiceType(), Is.EqualTo(typeof(TestSelfContainedService)));
            Assert.That(behaviorContext.GetMethodName(), Is.EqualTo(method.Name));

            var httpMethods = behaviorContext.GetSupportedHttpMethods().ToList();
            Assert.That(httpMethods.Contains(HttpMethod.Get), Is.True);
            Assert.That(httpMethods.Contains(HttpMethod.Head), Is.True);
            Assert.That(httpMethods.Contains(HttpMethod.Post), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Put), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Patch), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Delete), Is.False);
        }

        [Test]
        public void MethodExecutedContextShouldBePopulatedForSelfContainedService()
        {
            var service = new TestSelfContainedService();

            MethodInfo method = service.GetType().GetMethod("GetOK");
            Assert.That(method, Is.Not.Null);

            var returnedObject = String.Copy("OK");

            var behaviorContext = new MethodExecutedContext(service, method, returnedObject);
            Assert.That(behaviorContext.Service, Is.SameAs(service));
            Assert.That(behaviorContext.Method, Is.SameAs(method));
            Assert.That(behaviorContext.ReturnedObject, Is.SameAs(returnedObject));
            Assert.That(behaviorContext.GetServiceContractType(), Is.EqualTo(typeof(TestSelfContainedService)));
            Assert.That(behaviorContext.GetServiceType(), Is.EqualTo(typeof(TestSelfContainedService)));
            Assert.That(behaviorContext.GetMethodName(), Is.EqualTo(method.Name));

            var httpMethods = behaviorContext.GetSupportedHttpMethods().ToList();
            Assert.That(httpMethods.Contains(HttpMethod.Get), Is.True);
            Assert.That(httpMethods.Contains(HttpMethod.Head), Is.True);
            Assert.That(httpMethods.Contains(HttpMethod.Post), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Put), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Patch), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Delete), Is.False);
        }

        [Test]
        public void MethodAppliesContextShouldBePopulated()
        {
            var service = new TestService();

            MethodInfo method = service.GetType().GetMethod("GetAll");
            Assert.That(method, Is.Not.Null);

            var methodContext = new MethodAppliesContext(service, method);
            Assert.That(methodContext.Service, Is.SameAs(service));
            Assert.That(methodContext.Method, Is.SameAs(method));
            Assert.That(methodContext.GetServiceContractType(), Is.EqualTo(typeof(ITestService)));
            Assert.That(methodContext.GetServiceType(), Is.EqualTo(typeof(TestService)));
            Assert.That(methodContext.GetMethodName(), Is.EqualTo(method.Name));

            var httpMethods = methodContext.GetSupportedHttpMethods().ToList();
            Assert.That(httpMethods.Contains(HttpMethod.Get), Is.True);
            Assert.That(httpMethods.Contains(HttpMethod.Head), Is.True);
            Assert.That(httpMethods.Contains(HttpMethod.Post), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Put), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Patch), Is.False);
            Assert.That(httpMethods.Contains(HttpMethod.Delete), Is.False);
        }
    }
}
