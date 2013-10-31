using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace SimpleViewEngine.Controllers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal sealed class HttpGetHeadAttribute : ActionFilterAttribute
    {
        public const string AllowHeader = "Allow";

        private static readonly string[] allowedMethods = { "GET", "HEAD", "OPTIONS" };

        public static IEnumerable<string> AllowedMethods
        {
            get
            {
                return allowedMethods;
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            string httpMethodOverride = filterContext.HttpContext.Request.GetHttpMethodOverride();

            for (int i = 0; i < allowedMethods.Length; i++)
            {
                if (String.Equals(httpMethodOverride, allowedMethods[i], StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            filterContext.HttpContext.Response.AppendHeader(AllowHeader, String.Join(", ", allowedMethods));
            filterContext.Result = new HttpStatusCodeResult((int) HttpStatusCode.MethodNotAllowed);
        }
    }
}
