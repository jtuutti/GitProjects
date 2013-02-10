// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Net;
using System.Threading.Tasks;

namespace RestFoundation.Runtime
{
    internal static class TaskExceptionUnwrapper
    {
        public static Exception Unwrap(Task task)
        {
            AggregateException taskException = task.Exception;

            if (taskException == null)
            {
                return new HttpResponseException(HttpStatusCode.InternalServerError, RestResources.FailedRequest);
            }

            return ExceptionUnwrapper.IsDirectResponseException(taskException.InnerException) ? taskException.InnerException : taskException;
        }
    }
}
