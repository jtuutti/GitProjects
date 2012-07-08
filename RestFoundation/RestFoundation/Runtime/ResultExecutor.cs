using System;
using System.Net;

namespace RestFoundation.Runtime
{
    internal sealed class ResultExecutor
    {
        public void Execute(IServiceContext context, IResult result, Type returnType)
        {
            if (result != null)
            {
                result.Execute(context);
                DisposeIfNecessary(result);
            }
            else if (context.Response.GetStatusCode() == HttpStatusCode.OK && returnType == typeof(void))
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
