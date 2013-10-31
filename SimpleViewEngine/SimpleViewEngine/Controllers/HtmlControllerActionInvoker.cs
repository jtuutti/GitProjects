using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SimpleViewEngine.Controllers
{
    internal sealed class HtmlControllerActionInvoker : ControllerActionInvoker
    {
        private const string ActionNotFoundPartialMessage = "was not found";

        protected override ResultExecutedContext InvokeActionResultWithFilters(ControllerContext controllerContext,
                                                                               IList<IResultFilter> filters,
                                                                               ActionResult actionResult)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }

            try
            {
                return base.InvokeActionResultWithFilters(controllerContext, filters, actionResult);
            }
            catch (InvalidOperationException ex)
            {
                if (controllerContext.Controller is HtmlController && ex.Message.Contains(ActionNotFoundPartialMessage))
                {
                    return base.InvokeActionResultWithFilters(controllerContext, filters, new HttpNotFoundResult());
                }

                throw;
            }
        }
    }
}
