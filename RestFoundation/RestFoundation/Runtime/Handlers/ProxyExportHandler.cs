// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Routing;
using RestFoundation.ServiceProxy;

namespace RestFoundation.Runtime.Handlers
{
    internal sealed class ProxyExportHandler : IRouteHandler, IHttpHandler
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

            var session = new ProxySession
            {
                ServiceUrl = context.Request.Unvalidated.Form["ServiceUrl"],
                OperationUrl = context.Request.Unvalidated.Form["OperationUrl"],
                Format = context.Request.Unvalidated.Form["ResourceFormat"],
                Method = context.Request.Unvalidated.Form["HttpMethod"],
                Headers = context.Request.Unvalidated.Form["HeaderText"],
                Body = context.Request.Unvalidated.Form["RequestText"]
            };

            DateTime now = DateTime.Now;

            context.Response.Clear();        
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = "application/json";
            context.Response.AppendHeader("Content-Disposition", String.Format(
                                                                CultureInfo.InvariantCulture,
                                                                "attachment; filename=session.{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}{6:D3}.dat",
                                                                now.Year,
                                                                now.Month,
                                                                now.Day,
                                                                now.Hour,
                                                                now.Minute,
                                                                now.Second,
                                                                now.Millisecond));

            string serializedSession = ProxyJsonConvert.SerializeObject(session, false, false);

            context.Response.Output.Write(serializedSession);
            context.Response.Output.Flush();
        }
    }
}
