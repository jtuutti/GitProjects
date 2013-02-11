// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Net;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents the default result executor.
    /// </summary>
    public class ResultExecutor : IResultExecutor
    {
        /// <summary>
        /// Executes a no-content result in the provided service context.
        /// </summary>
        /// <param name="context">The service context.</param>
        public void ExecuteNoContent(IServiceContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (context.Response.GetStatusCode() != HttpStatusCode.OK)
            {
                return;
            }

            string currentStatusDescription = context.Response.GetStatusDescription();
            string statusDescription = !String.Equals(RestResources.OK, currentStatusDescription, StringComparison.OrdinalIgnoreCase) ?
                                           currentStatusDescription :
                                           RestResources.NoContent;

            context.Response.Output.Clear();
            context.Response.SetStatus(HttpStatusCode.NoContent, statusDescription);
        }

        /// <summary>
        /// Executes a result in the provided service context.
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

            if (result == null)
            {
                return;
            }

            result.Execute(context);
            DisposeIfNecessary(result);
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
