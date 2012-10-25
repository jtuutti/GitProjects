using System.Linq;
using NUnit.Framework;
using RestFoundation.Tests.Implementation.ServiceContracts;
using RestFoundation.Tests.Implementation.Services;

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
            Rest.Configuration
                .InitializeAndMock(builder =>
                {
                    builder.ScanAssemblies(new[] { GetType().Assembly }, t => t.Name.EndsWith("Service"));
                    builder.AllowPropertyInjection(type => type.GetProperties().Any(p => p.PropertyType == typeof(IServiceContext)));
                })
                .WithUrls(builder =>
                {
                    builder.MapUrl(TestServiceUrl).ToServiceContract<ITestService>();
                    builder.MapUrl(TestSelfContainedServiceUrl).ToServiceContract<TestSelfContainedService>();
                });
        }
    }
}
