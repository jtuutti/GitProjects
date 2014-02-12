// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Routing;

namespace RestFoundation.Runtime.Handlers
{
    internal class JavaScriptRouteHandler : IRouteHandler, IHttpHandler
    {
        private const string JavaScriptContentType = "application/x-javascript; charset=utf-8";

        private readonly string m_filename;

        public JavaScriptRouteHandler(string filename)
        {
            m_filename = filename;
        }

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

            using (Stream stream = typeof(Rest).Assembly.GetManifestResourceStream(String.Concat("RestFoundation.ServiceProxy.Resources.", m_filename)))
            {
                if (stream == null)
                {
                    throw new HttpException((int) HttpStatusCode.NotFound, Resources.Global.MissingJavaScriptResource);
                }

                context.Response.ContentType = JavaScriptContentType;
                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                context.Response.Cache.SetETag(GetAssemblyVersionAsEtag());
                context.Response.Cache.SetExpires(DateTime.Now.AddYears(1));
                context.Response.Cache.SetValidUntilExpires(true);

                stream.CopyTo(context.Response.OutputStream);
            }
        }

        private string GetAssemblyVersionAsEtag()
        {
            Version assemblyVersion = GetType().Assembly.GetName().Version;

            return String.Format(CultureInfo.InvariantCulture,
                                 "\"JS-{0}-{1}-{2}-{3}\"",
                                 assemblyVersion.Major,
                                 assemblyVersion.Minor,
                                 assemblyVersion.Build,
                                 assemblyVersion.Revision);
        }
    }
}
