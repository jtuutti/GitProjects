using System;
using MvcAlt.ViewModels;

namespace MvcAlt.Controllers
{
    public class FooController : IController
    {
        [RootUrl]
        [Url("foo/{id}", HttpVerb.Get)]
        public object Get(StringComparison? id)
        {
            return "OK: " + (id != null ? id.ToString() : "No Content");
        }
    }
}
