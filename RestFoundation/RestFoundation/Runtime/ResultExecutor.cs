using System;
using System.Net;

namespace RestFoundation.Runtime
{
    public class ResultExecutor : IResultExecutor
    {
        public virtual void Execute(IServiceContext context, IResult result, Type methodReturnType)
        {
            if (context == null) throw new ArgumentNullException("context");

            if (result != null)
            {
                result.Execute(context);
                DisposeIfNecessary(result);
            }
            else if (context.Response.GetStatusCode() == HttpStatusCode.OK && methodReturnType == typeof(void))
            {
                context.Response.SetStatus(HttpStatusCode.NoContent, "No Content");
            }
        }

        private static void DisposeIfNecessary(IResult result)
        {
            var disposableResult = result as IDisposable;

            if (disposableResult != null)
            {
                disposableResult.Dispose();
            }
        }
    }
}
