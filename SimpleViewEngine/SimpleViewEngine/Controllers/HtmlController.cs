using System;
using System.Net;
using System.Web.Mvc;

namespace SimpleViewEngine.Controllers
{
    /// <summary>
    /// Represents a controller that returns <see cref="HtmlView"/> instances based
    /// on routing rules.
    /// </summary>
    public class HtmlController : Controller
    {
        /// <summary>
        /// Called when a request matches this controller, but no method with the specified action name is found in the controller.
        /// </summary>
        /// <param name="actionName">The name of the attempted action.</param>
        protected override void HandleUnknownAction(string actionName)
        {
            if (String.IsNullOrEmpty(actionName))
            {
                var notFoundResult = new HttpStatusCodeResult(HttpStatusCode.NotFound);
                notFoundResult.ExecuteResult(ControllerContext);
                return;
            }

            if (!String.Equals(HttpVerbs.Get.ToString(), HttpContext.Request.HttpMethod, StringComparison.OrdinalIgnoreCase) &&
                !String.Equals(HttpVerbs.Head.ToString(), HttpContext.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
            {
                var notFoundResult = new HttpStatusCodeResult(HttpStatusCode.MethodNotAllowed);
                notFoundResult.ExecuteResult(ControllerContext);
                return;
            }

            try
            {
                View(actionName).ExecuteResult(ControllerContext);
            }
            catch (InvalidOperationException)
            {
                var notFoundResult = new HttpStatusCodeResult(HttpStatusCode.NotFound);
                notFoundResult.ExecuteResult(ControllerContext);
            }
        }
    }
}
