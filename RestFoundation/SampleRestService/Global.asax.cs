using System;
using System.Web;
using RestFoundation;
using SampleRestService.Contracts;

namespace SampleRestService
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            Rest.Configuration
                .Initialize(typeof(Global).Assembly)
                .ConfigureServiceHelpAndProxy(c => c.Enable())
                .WithUrls(RegisterUrls);
        }

        private static void RegisterUrls(UrlBuilder builder)
        {
            builder.MapUrl("sample").ToServiceContract<ISampleService>();
        }
    }
}
