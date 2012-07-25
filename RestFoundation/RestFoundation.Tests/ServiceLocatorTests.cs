using System;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using RestFoundation.Context;
using RestFoundation.Runtime;
using RestFoundation.Runtime.Handlers;
using RestFoundation.Security;
using RestFoundation.ServiceLocation;
using RestFoundation.Tests.ServiceContracts;
using RestFoundation.Tests.Services;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests
{
    [TestFixture]
    public class ServiceLocatorTests
    {
        [Test]
        public void StructureMap_TestIfSystemInterfacesAreRegisteredAndHaveCorrectLifetime()
        {
            using (IServiceLocator serviceLocator = Rest.Active.CreateServiceLocatorForStructureMap(false))
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
                TestTransient<IRestAsyncHandler>(serviceLocator);
                TestTransient<IRestHandler>(serviceLocator);
                TestTransient<IServiceContext>(serviceLocator);
                TestTransient<IServiceMethodInvoker>(serviceLocator);
                TestTransient<IServiceMethodLocator>(serviceLocator);
            }
        }

        [Test]
        public void StructureMap_TestIfSystemInterfacesHaveCorrectImplementations()
        {
            using (IServiceLocator serviceLocator = Rest.Active.CreateServiceLocatorForStructureMap(false))
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
                TestImplementation<IRestAsyncHandler, RestAsyncHandler>(serviceLocator);
                TestImplementation<IRestHandler, RestHandler>(serviceLocator);
                TestImplementation<IServiceContext, ServiceContext>(serviceLocator);
                TestImplementation<IServiceMethodInvoker, ServiceMethodInvoker>(serviceLocator);
                TestImplementation<IServiceMethodLocator, ServiceMethodLocator>(serviceLocator);
            }
        }

        [Test]
        public void StructureMap_TestInjectedImplementation()
        {
            using (IServiceLocator serviceLocator = Rest.Active.CreateServiceLocatorForStructureMap(builder =>
            {
                builder.For<IAuthorizationManager>().Transient().Use<TestAuthorizationManager>();
            }, false))
            {
                TestTransient<IAuthorizationManager>(serviceLocator);
                TestImplementation<IAuthorizationManager, TestAuthorizationManager>(serviceLocator);
            }
        }

        [Test]
        public void StructureMap_TestServiceResolutionWithoutRegistration()
        {
            using (IServiceLocator serviceLocator = Rest.Active.CreateServiceLocatorForStructureMap(true))
            {
                var service = serviceLocator.GetService<ITestService>();
                Assert.That(service, Is.Null, "A service was resolved without any registration");
            }
        }

        [Test]
        public void StructureMap_TestServiceResolution()
        {
            using (IServiceLocator serviceLocator = Rest.Active.CreateServiceLocatorForStructureMap(builder =>
            {
                builder.For<ITestService>().Use<TestService>();
            }, true))
            {
                TestTransient<ITestService>(serviceLocator);
                TestImplementation<ITestService, TestService>(serviceLocator);
            }
        }

        [Test]
        public void StructureMap_TestServiceResolutionRegisteredByConvention()
        {
            using (IServiceLocator serviceLocator = Rest.Active.CreateServiceLocatorForStructureMap(builder =>
            {
                builder.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.WithDefaultConventions();
                });
            }, true))
            {
                TestTransient<ITestService>(serviceLocator);

                var service = (TestService) TestImplementation<ITestService, TestService>(serviceLocator);
                Assert.That(service.Context, Is.Not.Null, "StructureMap did not inject REST dependencies as properties");
            }
        }

        [Test]
        public void Unity_TestIfSystemInterfacesAreRegisteredAndHaveCorrectLifetime()
        {
            using (IServiceLocator serviceLocator = Rest.Active.CreateServiceLocatorForUnity(false))
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
                TestTransient<IRestAsyncHandler>(serviceLocator);
                TestTransient<IRestHandler>(serviceLocator);
                TestTransient<IServiceContext>(serviceLocator);
                TestTransient<IServiceMethodInvoker>(serviceLocator);
                TestTransient<IServiceMethodLocator>(serviceLocator);
            }
        }
    
        [Test]
        public void Unity_TestIfSystemInterfacesHaveCorrectImplementations()
        {
            using (IServiceLocator serviceLocator = Rest.Active.CreateServiceLocatorForUnity(false))
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
                TestImplementation<IRestAsyncHandler, RestAsyncHandler>(serviceLocator);
                TestImplementation<IRestHandler, RestHandler>(serviceLocator);
                TestImplementation<IServiceContext, ServiceContext>(serviceLocator);
                TestImplementation<IServiceMethodInvoker, ServiceMethodInvoker>(serviceLocator);
                TestImplementation<IServiceMethodLocator, ServiceMethodLocator>(serviceLocator);
            }
        }

        [Test]
        public void Unity_TestInjectedImplementation()
        {
            using (IServiceLocator serviceLocator = Rest.Active.CreateServiceLocatorForUnity(builder =>
            {
                builder.RegisterType<IAuthorizationManager, TestAuthorizationManager>(new TransientLifetimeManager());
            }, false))
            {
                TestTransient<IAuthorizationManager>(serviceLocator);
                TestImplementation<IAuthorizationManager, TestAuthorizationManager>(serviceLocator);
            }
        }

        [Test]
        public void Unity_TestServiceResolutionWithoutRegistration()
        {
            using (IServiceLocator serviceLocator = Rest.Active.CreateServiceLocatorForUnity(true))
            {
                var service = serviceLocator.GetService<ITestService>();
                Assert.That(service, Is.Null, "A service was resolved without any registration");
            }
        }

        [Test]
        public void Unity_TestServiceResolution()
        {
            using (IServiceLocator serviceLocator = Rest.Active.CreateServiceLocatorForUnity(builder =>
            {
                builder.RegisterType<ITestService, TestService>(new InjectionProperty("Context")); // property injection can only be specified manually for Unity
            }, true))
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
