// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Net;
using RestFoundation.Results;

namespace RestFoundation.Runtime
{
    internal sealed class ResultExecutor
    {
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

        public void Execute(IResult result, IServiceContext context)
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
