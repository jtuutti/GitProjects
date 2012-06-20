using System.Net;

namespace RestFoundation.Results
{
    public class RedirectResult : IResult
    {
        public IHttpRequest Request { get; set; }
        public IHttpResponse Response { get; set; }
        public virtual bool IsPermanent { get; set; }
        public virtual string RedirectUrl { get; set; }

        public void Execute()
        {
            if (Response == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No HTTP context found");
            }

            Response.Redirect(RedirectUrl, false, false);
        }
    }
}
