using System;
using System.Globalization;
using System.Web;
using System.Web.Routing;

namespace RestFoundation.Runtime.Handlers
{
    internal class CssRouteHandler : IRouteHandler, IHttpHandler
    {
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this;
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            using (var jqueryStream = typeof(Rest).Assembly.GetManifestResourceStream("RestFoundation.ServiceProxy.Resources.help.css"))
            {
                if (jqueryStream == null)
                {
                    throw new HttpException(404, "CSS resource not found");
                }

                context.Response.ContentType = "text/css";
                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                context.Response.Cache.SetETag(GetAssemblyVersionAsEtag());
                context.Response.Cache.SetExpires(DateTime.Now.AddYears(1));
                context.Response.Cache.SetValidUntilExpires(true);

                jqueryStream.CopyTo(context.Response.OutputStream);
            }
        }

        private string GetAssemblyVersionAsEtag()
        {
            Version assemblyVersion = GetType().Assembly.GetName().Version;

            return String.Format(CultureInfo.InvariantCulture,
                                 "\"CSS-{0}-{1}-{2}-{3}\"",
                                 assemblyVersion.Major,
                                 assemblyVersion.Minor,
                                 assemblyVersion.Build,
                                 assemblyVersion.Revision);
        }
    }
}
