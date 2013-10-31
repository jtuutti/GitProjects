using System;
using System.Net;
using System.Web.Mvc;

namespace SimpleViewEngine.Controllers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal sealed class HttpGetHeadAttribute : ActionFilterAttribute
    {
        public const string AllowHeader = "Allow";

        private static readonly string[] allowedMethodArray = { "GET", "HEAD", "OPTIONS" };
        private static readonly object syncRoot = new Object();
        private static string allowedMethods;

        public static string AllowedMethods
        {
            get
            {
                lock (syncRoot)
                {
                    return allowedMethods ?? (allowedMethods = String.Join(", ", allowedMethodArray));
                }
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            string httpMethodOverride = filterContext.HttpContext.Request.GetHttpMethodOverride();

            for (int i = 0; i < allowedMethodArray.Length; i++)
            {
                if (String.Equals(httpMethodOverride, allowedMethodArray[i], StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            filterContext.HttpContext.Response.AppendHeader(AllowHeader, AllowedMethods);
            filterContext.Result = new HttpStatusCodeResult((int) HttpStatusCode.MethodNotAllowed);
        }
    }
}
