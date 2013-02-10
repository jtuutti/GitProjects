using System;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using RestFoundation.Context;
using RestFoundation.Results;
using RestFoundation.Runtime;
using RestFoundation.Runtime.Handlers;
using RestFoundation.Security;
using RestFoundation.ServiceLocation;
using RestFoundation.Tests.Implementation.Authorization;
using RestFoundation.Tests.Implementation.ServiceContracts;
using RestFoundation.Tests.Implementation.Services;
using RestFoundation.UnitTesting;
using RestFoundation.Validation;
using StructureMap;

namespace RestFoundation.Tests.IoC
{
    [TestFixture]
    public class ServiceLocatorTests
    {
        [Test]
        public void StructureMap_TestIfSystemInterfacesAreRegisteredAndHaveCorrectLifetime()
        {
            using (IServiceLocator serviceLocator = Rest.Configuration.CreateServiceLocatorForStructureMap(false))
            {
                TestSingleton<IAuthorizationManager>(serviceLocator);
                TestSingleton<IContentNegotiator>(serviceLocator);
                TestSingleton<IHttpMethodResolver>(serviceLocator);
                TestSingleton<IParameterValueProvider>(serviceLocator);
                TestSingleton<IResourceValidator>(serviceLocator);
                TestSingleton<IResultExecutor>(serviceLocator);
                TestSingleton<IResultFactory>(serviceLocator);
                TestSingleton<IServiceCache>(serviceLocator);
                TestSingleton<IStreamCompressor>(serviceLocator);

                TestTransient<IHttpRequest>(serviceLocator);
                TestTransient<IHttpResponse>(serviceLocator);
                TestTransient<IHttpResponseOutput>(serviceLocator);
                TestTransient<IRestServiceHandler>(serviceLocator);
                TestTransient<IServiceContext>(serviceLocator);
                TestTransient<IServiceMethodInvoker>(serviceLocator);
                TestTransient<IServiceMethodLocator>(serviceLocator);
            }
        }

        [Test]
        public void StructureMap_TestIfSystemInterfacesHaveCorrectImplementations()
        {
            using (IServiceLocator serviceLocator = Rest.Configuration.CreateServiceLocatorForStructureMap(false))
            {
                TestImplementation<IAuthorizationManager, AuthorizationManager>(serviceLocator);
                TestImplementation<IContentNegotiator, ContentNegotiator>(serviceLocator);
                TestImplementation<IHttpMethodResolver, HttpMethodResolver>(serviceLocator);
                TestImplementation<IParameterValueProvider, ParameterValueProvider>(serviceLocator);
                TestImplementation<IResourceValidator, ResourceValidator>(serviceLocator);
                TestImplementation<IResultExecutor, ResultExecutor>(serviceLocator);
                TestImplementation<IResultFactory, ResultFactory>(serviceLocator);
                TestImplementation<IServiceCache, ServiceCache>(serviceLocator);
                TestImplementation<IStreamCompressor, StreamCompressor>(serviceLocator);
                TestImplementation<IHttpRequest, HttpRequest>(serviceLocator);
                TestImplementation<IHttpResponse, HttpResponse>(serviceLocator);
                TestImplementation<IHttpResponseOutput, HttpResponseOutput>(serviceLocator);
                TestImplementation<IRestServiceHandler, RestServiceHandler>(serviceLocator);
                TestImplementation<IServiceContext, ServiceContext>(serviceLocator);
                TestImplementation<IServiceMethodInvoker, ServiceMethodInvoker>(serviceLocator);
                TestImplementation<IServiceMethodLocator, ServiceMethodLocator>(serviceLocator);
            }
        }

        [Test]
        public void StructureMap_TestInjectedImplementation()
        {
            var container = new Container(registry => registry.For<IAuthorizationManager>().Use<TestAuthorizationManager>());

            using (IServiceLocator serviceLocator = Rest.Configuration.CreateServiceLocatorForStructureMap(container, false))
            {
                TestTransient<IAuthorizationManager>(serviceLocator);
                TestImplementation<IAuthorizationManager, TestAuthorizationManager>(serviceLocator);
            }
        }

        [Test]
        public void StructureMap_TestServiceResolutionWithoutRegistration()
        {
            using (IServiceLocator serviceLocator = Rest.Configuration.CreateServiceLocatorForStructureMap(true))
            {
                var service = serviceLocator.GetService<ITestService>();
                Assert.That(service, Is.Null, "A service was resolved without any registration");
            }
        }

        [Test]
        public void StructureMap_TestServiceResolution()
        {
            var container = new Container(registry =>
            {
                registry.For<ITestService>().Use<TestService>();
                registry.SetAllProperties(convention => convention.TypeMatches(t => t == typeof(IServiceContext)));
            });

            using (IServiceLocator serviceLocator = Rest.Configuration.CreateServiceLocatorForStructureMap(container, true))
            {
                TestTransient<ITestService>(serviceLocator);

                var service = (TestService) TestImplementation<ITestService, TestService>(serviceLocator);
                Assert.That(service.Context, Is.Not.Null, "StructureMap did not inject REST dependencies as properties");
            }
        }

        [Test]
        public void StructureMap_TestServiceResolutionRegisteredByConvention()
        {
            var container = new Container(registry =>
            {
                registry.Scan(action =>
                {
                    action.TheCallingAssembly();
                    action.WithDefaultConventions();
                });
                registry.SetAllProperties(convention => convention.TypeMatches(t => t == typeof(IServiceContext)));
            });

            using (IServiceLocator serviceLocator = Rest.Configuration.CreateServiceLocatorForStructureMap(container, true))
            {
                TestTransient<ITestService>(serviceLocator);

                var service = (TestService) TestImplementation<ITestService, TestService>(serviceLocator);
                Assert.That(service.Context, Is.Not.Null, "StructureMap did not inject REST dependencies as properties");
            }
        }

        [Test]
        public void Unity_TestIfSystemInterfacesAreRegisteredAndHaveCorrectLifetime()
        {
            using (IServiceLocator serviceLocator = Rest.Configuration.CreateServiceLocatorForUnity(false))
            {
                TestSingleton<IAuthorizationManager>(serviceLocator);
                TestSingleton<IContentNegotiator>(serviceLocator);
                TestSingleton<IHttpMethodResolver>(serviceLocator);
                TestSingleton<IParameterValueProvider>(serviceLocator);
                TestSingleton<IResourceValidator>(serviceLocator);
                TestSingleton<IResultExecutor>(serviceLocator);
                TestSingleton<IResultFactory>(serviceLocator);
                TestSingleton<IServiceCache>(serviceLocator);
                TestSingleton<IStreamCompressor>(serviceLocator);

                TestTransient<IHttpRequest>(serviceLocator);
                TestTransient<IHttpResponse>(serviceLocator);
                TestTransient<IHttpResponseOutput>(serviceLocator);
                TestTransient<IRestServiceHandler>(serviceLocator);
                TestTransient<IServiceContext>(serviceLocator);
                TestTransient<IServiceMethodInvoker>(serviceLocator);
                TestTransient<IServiceMethodLocator>(serviceLocator);
            }
        }
    
        [Test]
        public void Unity_TestIfSystemInterfacesHaveCorrectImplementations()
        {
            using (IServiceLocator serviceLocator = Rest.Configuration.CreateServiceLocatorForUnity(false))
            {
                TestImplementation<IAuthorizationManager, AuthorizationManager>(serviceLocator);
                TestImplementation<IContentNegotiator, ContentNegotiator>(serviceLocator);
                TestImplementation<IHttpMethodResolver, HttpMethodResolver>(serviceLocator);
                TestImplementation<IParameterValueProvider, ParameterValueProvider>(serviceLocator);
                TestImplementation<IResourceValidator, ResourceValidator>(serviceLocator);
                TestImplementation<IResultExecutor, ResultExecutor>(serviceLocator);
                TestImplementation<IResultFactory, ResultFactory>(serviceLocator);
                TestImplementation<IServiceCache, ServiceCache>(serviceLocator);
                TestImplementation<IStreamCompressor, StreamCompressor>(serviceLocator);
                TestImplementation<IHttpRequest, HttpRequest>(serviceLocator);
                TestImplementation<IHttpResponse, HttpResponse>(serviceLocator);
                TestImplementation<IHttpResponseOutput, HttpResponseOutput>(serviceLocator);
                TestImplementation<IRestServiceHandler, RestServiceHandler>(serviceLocator);
                TestImplementation<IServiceContext, ServiceContext>(serviceLocator);
                TestImplementation<IServiceMethodInvoker, ServiceMethodInvoker>(serviceLocator);
                TestImplementation<IServiceMethodLocator, ServiceMethodLocator>(serviceLocator);
            }
        }

        [Test]
        public void Unity_TestInjectedImplementation()
        {
            var container = new UnityContainer();
            container.RegisterType<IAuthorizationManager, TestAuthorizationManager>(new TransientLifetimeManager());

            using (IServiceLocator serviceLocator = Rest.Configuration.CreateServiceLocatorForUnity(container, false))
            {
                TestTransient<IAuthorizationManager>(serviceLocator);
                TestImplementation<IAuthorizationManager, TestAuthorizationManager>(serviceLocator);
            }
        }

        [Test]
        public void Unity_TestServiceResolutionWithoutRegistration()
        {
            using (IServiceLocator serviceLocator = Rest.Configuration.CreateServiceLocatorForUnity(true))
            {
                var service = serviceLocator.GetService<ITestService>();
                Assert.That(service, Is.Null, "A service was resolved without any registration");
            }
        }

        [Test]
        public void Unity_TestServiceResolution()
        {
            var container = new UnityContainer();
            container.RegisterType<ITestService, TestService>(new InjectionProperty("Context")); // property injection can only be specified manually for Unity

            using (IServiceLocator serviceLocator = Rest.Configuration.CreateServiceLocatorForUnity(container, true))
            {
                TestTransient<ITestService>(serviceLocator);

                var service = (TestService) TestImplementation<ITestService, TestService>(serviceLocator);
                Assert.That(service.Context, Is.Not.Null, "Unity did not inject REST dependencies as properties");
            }
        }

        [Test, Ignore]
        public void Unity_TestServiceResolutionRegisteredByConvention()
        {
            Assert.Inconclusive("Unity does not support registrations by convention");
        }

        private static void TestSingleton<T>(IServiceLocator serviceLocator)
        {
            var service1 = serviceLocator.GetService<T>();
            var service2 = serviceLocator.GetService<T>();

            Assert.That(service1, Is.Not.Null, String.Format("Service dependency of type '{0}' is not registered", typeof(T).FullName));
            Assert.That(service2, Is.SameAs(service1), String.Format("Service dependency of type '{0}' is not a singleton", typeof(T).FullName));
        }

        private static void TestTransient<T>(IServiceLocator serviceLocator)
        {
            var service1 = serviceLocator.GetService<T>();
            var service2 = serviceLocator.GetService<T>();

            Assert.That(service1, Is.Not.Null, String.Format("Service dependency of type '{0}' is not registered", typeof(T).FullName));
            Assert.That(service2, Is.Not.SameAs(service1), String.Format("Service dependency of type '{0}' should not be registered as a singleton", typeof(T).FullName));
        }

        public static TAbstraction TestImplementation<TAbstraction, TImplementation>(IServiceLocator serviceLocator)
            where TImplementation : TAbstraction
        {
            var service = serviceLocator.GetService<TAbstraction>();

            Assert.That(service, Is.Not.Null, String.Format("Service dependency of type '{0}' is not registered", typeof(TAbstraction).FullName));
            Assert.That(service, Is.TypeOf<TImplementation>(), String.Format("Service dependency of type '{0}' does not resolve to an expected implementation of type '{1}'",
                                                                             typeof(TAbstraction).FullName,
                                                                             typeof(TImplementation).FullName));

            return service;
        }
    }
}
