using System;
using System.Net;
using RestFoundation.Results;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents the default result executor.
    /// </summary>
    public class ResultExecutor : IResultExecutor
    {
        /// <summary>
        /// Executes the result in the provided service context.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="methodReturnType">The service method return type.</param>
        /// <param name="context">The service context.</param>
        public virtual void Execute(IResult result, Type methodReturnType, IServiceContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

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
