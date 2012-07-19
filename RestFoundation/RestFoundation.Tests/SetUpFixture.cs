using NUnit.Framework;
using RestFoundation.Behaviors;
using RestFoundation.Tests.ServiceContracts;
using RestFoundation.Tests.Services;
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
                       .WithRoutes(RegisterRoutes);
        }

        private static void RegisterDependencies(Registry registry)
        {
            registry.For<ITestService>().Use<TestService>();
        }

        private static void RegisterRoutes(RouteBuilder routeBuilder)
        {
            routeBuilder.MapRestRoute<ITestService>(RelativeUrl).WithBehaviors(new HttpsOnlyBehavior());
        }
    }
}
