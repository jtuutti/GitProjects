using RestFoundation.Configuration;
using SampleRestService.Contracts;

namespace SampleRestService.App_Start
{
    public static class RestConfig
    {
        public static void Initialize()
        {
            RestFoundation.Rest.Configuration
                               .Initialize(typeof(Global).Assembly)
                               .ConfigureServiceHelpAndProxy(c => c.Enable().WithServiceDescription("A sample REST service"))
                               .WithUrls(RegisterUrls);
        }

        private static void RegisterUrls(UrlBuilder builder)
        {
            builder.MapUrl("sample").ToServiceContract<ISampleService>();
        }
    }
}
