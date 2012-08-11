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
        public const string RelativeUrl = "test-service";

        [SetUp]
        public void Setup()
        {
            // Configuring REST foundation
            Rest.Active.ConfigureMocksWithStructureMap(CreateContainerWithDependencies())
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
            urlBuilder.MapUrl(RelativeUrl).ToServiceContract<ITestService>();
        }
    }
}
