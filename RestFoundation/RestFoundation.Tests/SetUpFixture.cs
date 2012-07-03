using System;
using NUnit.Framework;
using RestFoundation.Test;
using RestFoundation.Tests.ServiceContracts;
using StructureMap;

namespace RestFoundation.Tests
{
    [SetUpFixture]
    public class SetUpFixture
    {
        public const string RelativeUrl = "test-service";

        [SetUp]
        public void Setup()
        {
            // StructureMap IoC container configuration
            ObjectFactory.Configure(ConfigureIoC);
            ObjectFactory.AssertConfigurationIsValid();

            // Configuring REST foundation
            Rest.Configure.WithObjectFactory(CreateObjectFactory)
                          .WithRoutes(RegisterRoutes);
        }

        private static void ConfigureIoC(ConfigurationExpression config)
        {
            config.Scan(action =>
                        {
                            action.Assembly(ServiceAssembly.Executing);
                            action.TheCallingAssembly();
                            action.WithDefaultConventions();
                        });

            config.For<IServiceContext>().Use<MockServiceContext>();
            config.For<IHttpRequest>().Use<MockHttpRequest>();
            config.For<IHttpResponse>().Use<MockHttpResponse>();
            config.For<IHttpResponseOutput>().Use<MockHttpResponseOutput>();

            config.SetAllProperties(convention => convention.TypeMatches(type => type.IsRestDependency()));
        }

        private static object CreateObjectFactory(Type type)
        {
            if (type.IsInterface || type.IsAbstract)
            {
                return ObjectFactory.TryGetInstance(type);
            }

            return ObjectFactory.GetInstance(type);
        }

        private static void RegisterRoutes(RouteBuilder routeBuilder)
        {
            routeBuilder.MapRestRoute<ITestService>(RelativeUrl);
        }
    }
}
