using System.Web;

namespace RestFoundation.UnitTesting
{
    internal sealed class TestHttpApplication : HttpApplication
    {
        public TestHttpApplication()
        {
            RestHttpModule.IsInitialized = true;
        }
    }
}
