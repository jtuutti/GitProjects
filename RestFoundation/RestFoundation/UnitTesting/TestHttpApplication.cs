using System.Reflection;
using System.Web;

namespace RestFoundation.UnitTesting
{
    internal sealed class TestHttpApplication : HttpApplication
    {
        public TestHttpApplication()
        {
            MethodInfo addMethod = Modules.GetType().GetMethod("AddModule", BindingFlags.Instance | BindingFlags.NonPublic);

            if (addMethod != null)
            {
                addMethod.Invoke(Modules, new object[] { "HttpResponseModule", new HttpResponseModule() });
            }
        }
    }
}
