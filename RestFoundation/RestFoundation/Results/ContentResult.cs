using System;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    public class ContentResult : IResult
    {
        public ContentResult()
        {
            ClearOutput = true;
        }

        public string Content { get; set; }
        public string ContentType { get; set; }
        public bool ClearOutput { get; set; }

        public virtual void Execute(IServiceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            if (Content == null)
            {
                return;
            }

            if (ClearOutput)
            {
                context.Response.Output.Clear();
            }

            if (!String.IsNullOrEmpty(ContentType))
            {
                context.Response.SetHeader("Content-Type", ContentType);
            }
            else
            {
                string acceptType = context.Request.GetPreferredAcceptType();

                if (!String.IsNullOrEmpty(acceptType))
                {
                    context.Response.SetHeader("Content-Type", acceptType);
                }
            }

            context.Response.SetCharsetEncoding(context.Request.Headers.AcceptCharsetEncoding);
            
            OutputCompressionManager.FilterResponse(context);

            context.Response.Output.Write(Content);
        }
    }
}
