using System.Web.Mvc;

namespace SimpleViewEngine.Controllers
{
    /// <summary>
    /// Represents a controller that returns <see cref="HtmlView"/> instances and
    /// also provides support for HEAD and OPTIONS HTTP methods.
    /// </summary>
    public class HtmlController : Controller
    {
        /// <summary>
        /// Returns an HTML view based on routing.
        /// </summary>
        /// <returns>A view result.</returns>
        [HttpGetHead]
        public ViewResult Index()
        {
            return View();
        }

        /// <summary>
        /// Returns a list of allowed HTTP methods in the HTTP Allow header.
        /// </summary>
        /// <returns>An empty result.</returns>
        [HttpOptions, ActionName("Index")]
        public EmptyResult Options()
        {
            Response.AppendHeader("Allow", "GET, HEAD, OPTIONS");

            return new EmptyResult();
        }
    }
}
