using System;
using System.Web;
using SampleRestService.App_Start;

namespace SampleRestService
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RestConfig.Initialize();
        }
    }
}
