using System;
using System.Web.Mvc;

namespace SimpleViewEngine.Controllers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal sealed class HttpGetHeadAttribute : ActionFilterAttribute
    {
        private readonly string[] m_allowedMethods;

        public HttpGetHeadAttribute()
        {
            m_allowedMethods = new[] { "GET", "HEAD", "OPTIONS" };
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            string httpMethodOverride = filterContext.HttpContext.Request.GetHttpMethodOverride();

            for (int i = 0; i < m_allowedMethods.Length; i++)
            {
                if (String.Equals(httpMethodOverride, m_allowedMethods[i], StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            filterContext.HttpContext.Response.AppendHeader("Allow", String.Join(", ", m_allowedMethods).ToUpperInvariant());
            filterContext.Result = new HttpStatusCodeResult(405);
        }
    }
}
