// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
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
