using NUnit.Framework;
using RestFoundation.Tests.Implementation.ServiceContracts;
using RestFoundation.Tests.Implementation.Services;
using RestFoundation.UnitTesting;
using StructureMap;

namespace RestFoundation.Tests
{
    [SetUpFixture]
    public class SetUpFixture
    {
        public const string TestServiceUrl = "test-service";
        public const string TestSelfContainedServiceUrl = "test-ok-fail";

        [SetUp]
        public void Setup()
        {
            // Configuring REST foundation
            Rest.Configuration
                .InitializeAndMockWithStructureMap(CreateContainerWithDependencies())
                .WithUrls(RegisterUrls);
        }

        private static IContainer CreateContainerWithDependencies()
        {
            return new Container(registry =>
            {
                registry.For<ITestService>().Use<TestService>();
                registry.SetAllProperties(convention => convention.TypeMatches(type => type == typeof(IServiceContext)));
            });
        }

        private static void RegisterUrls(UrlBuilder urlBuilder)
        {
            urlBuilder.MapUrl(TestServiceUrl).ToServiceContract<ITestService>();
            urlBuilder.MapUrl(TestSelfContainedServiceUrl).ToServiceContract<TestSelfContainedService>();
        }
    }
}
