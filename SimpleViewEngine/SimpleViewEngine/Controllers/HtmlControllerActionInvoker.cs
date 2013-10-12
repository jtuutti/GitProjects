using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SimpleViewEngine.Controllers
{
    internal sealed class HtmlControllerActionInvoker : ControllerActionInvoker
    {
        protected override ResultExecutedContext InvokeActionResultWithFilters(ControllerContext controllerContext,
                                                                               IList<IResultFilter> filters,
                                                                               ActionResult actionResult)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }

            if (!(controllerContext.Controller is HtmlController))
            {
                return base.InvokeActionResultWithFilters(controllerContext, filters, actionResult);
            }

            try
            {
                return base.InvokeActionResultWithFilters(controllerContext, filters, actionResult);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("was not found"))
                {
                    return base.InvokeActionResultWithFilters(controllerContext, filters, new HttpNotFoundResult());
                }

                throw;
            }
        }
    }
}
