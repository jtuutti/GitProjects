using RestFoundation;
using RestFoundation.Configuration;
using SampleRestService.Contracts;
using SampleRestService.UrlRewriter;

namespace SampleRestService.App_Start
{
    public static class RestConfig
    {
        public static void Initialize()
        {
            Rest.Configuration
                .Initialize(RegisterServiceDependencies)
                .ConfigureServiceHelpAndProxy(c => c.Enable().WithServiceDescription("A sample REST service"))
                .WithUrls(RegisterUrls);
        }

        private static void RegisterServiceDependencies(DependencyBuilder builder)
        {
            builder.Register<IUrlRewriter, LegacyUrlRewriter>(InstanceLifetime.Singleton)
                   .ScanAssemblies(typeof(Global).Assembly);
        }

        private static void RegisterUrls(UrlBuilder builder)
        {
            builder.MapUrl("sample").ToServiceContract<ISampleService>();
        }
    }
}
