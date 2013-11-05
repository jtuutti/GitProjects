using System;
using System.Web.Mvc;

namespace SimpleViewEngine.Example.Controllers
{
    public class HomeAltController : Controller
    {
        [HttpGet]
        public ViewResult Index()
        {
            return View(new { user = "Joe", role = "Sales", timestamp = DateTime.UtcNow });
        }
    }
}
