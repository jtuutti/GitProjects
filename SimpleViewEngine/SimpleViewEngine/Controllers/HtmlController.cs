using System;
using System.Net;
using System.Web.Mvc;
using System.Web.SessionState;

namespace SimpleViewEngine.Controllers
{
    /// <summary>
    /// Represents a controller that returns <see cref="HtmlView"/> instances based
    /// on routing rules.
    /// </summary>
    [SessionState(SessionStateBehavior.Disabled)]
    public class HtmlController : Controller
    {
        private const string ActionNotFoundPartialMessage = "was not found";

        /// <summary>
        /// Renders the view based on the routing data.
        /// </summary>
        /// <returns>The rendered view.</returns>
        [HttpGet]
        public ViewResult Render()
        {
            return View();
        }

        /// <summary>
        /// Called when a request matches this controller, but no method with the specified action name is found in the controller.
        /// </summary>
        /// <param name="actionName">The name of the attempted action.</param>
        protected override void HandleUnknownAction(string actionName)
        {
            if (!String.Equals(HttpVerbs.Get.ToString(), HttpContext.Request.HttpMethod, StringComparison.OrdinalIgnoreCase) &&
                !String.Equals(HttpVerbs.Head.ToString(), HttpContext.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
            {
                var methodNotAllowedResult = new HttpStatusCodeResult(HttpStatusCode.MethodNotAllowed);
                methodNotAllowedResult.ExecuteResult(ControllerContext);
                return;
            }

            try
            {
                if (!ActionInvoker.InvokeAction(ControllerContext, "Render"))
                {
                    GenerateNotFoundStatus();
                }
            }
            catch (InvalidOperationException ex)
            {
                if (!ex.Message.Contains(ActionNotFoundPartialMessage))
                {
                    throw;
                }

                GenerateNotFoundStatus();
            }
        }

        private void GenerateNotFoundStatus()
        {
            var notFoundResult = new HttpStatusCodeResult(HttpStatusCode.NotFound);
            notFoundResult.ExecuteResult(ControllerContext);
        }
    }
}
