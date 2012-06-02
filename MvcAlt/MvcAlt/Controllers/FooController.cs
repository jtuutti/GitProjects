using MvcAlt.ViewModels;

namespace MvcAlt.Controllers
{
    public class FooController : IController
    {
        [Url("foo/{id}", HttpVerb.Get)]
        public object Get(Input foo)
        {
            return "OK: " + (foo != null ? foo.ToString() : "No Content");
        }
    }
}
