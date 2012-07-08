using System;
using System.Net;

namespace RestFoundation.Results
{
    public class RedirectResult : IResult
    {
        public virtual bool IsPermanent { get; set; }
        public virtual string RedirectUrl { get; set; }

        public virtual void Execute(IServiceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            context.Response.SetHeader(context.Response.Headers.Location, RedirectUrl);
            context.Response.SetStatus(IsPermanent ? HttpStatusCode.MovedPermanently : HttpStatusCode.Redirect);
        }
    }
}
