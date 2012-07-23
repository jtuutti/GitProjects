using NUnit.Framework;
using RestFoundation.Tests.ServiceContracts;
using RestFoundation.Tests.Services;
using RestFoundation.UnitTesting;
using StructureMap.Configuration.DSL;

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
            Rest.Active.ConfigureMocksWithStructureMap(RegisterDependencies)
                       .WithUrls(RegisterUrls);
        }

        private static void RegisterDependencies(Registry registry)
        {
            registry.For<ITestService>().Use<TestService>();
        }

        private static void RegisterUrls(UrlBuilder urlBuilder)
        {
            urlBuilder.MapUrl(RelativeUrl).ToServiceContract<ITestService>();
        }
    }
}
