using System.Web.Mvc;

namespace SimpleViewEngine.Example.Controllers
{
    public class CopyrightController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return PartialView("footer");
        }
    }
}
