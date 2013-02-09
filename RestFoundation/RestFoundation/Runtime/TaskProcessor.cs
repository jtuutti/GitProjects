// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace RestFoundation.Runtime
{
    internal static class TaskProcessor
    {
        public static object Complete(Task task, TimeSpan? timeout)
        {
            object returnedObj = null;
            Exception taskException = null;

            var waitHandler = new ManualResetEvent(false);

            task.ContinueWith(t =>
            {
                try
                {
                    if (t.IsFaulted && t.Exception != null)
                    {
                        taskException = t.Exception.InnerException;
                        return;
                    }

                    if (t.IsCanceled)
                    {
                        taskException = new HttpResponseException(HttpStatusCode.ServiceUnavailable, RestResources.ServiceUnavailable);
                        return;
                    }

                    PropertyInfo resultProperty = t.GetType().GetProperty("Result");

                    if (resultProperty != null)
                    {
                        returnedObj = resultProperty.GetValue(t, null);
                    }
                }
                finally
                {
                    waitHandler.Set();
                }
            });

            Wait(waitHandler, timeout);

            if (taskException != null)
            {
                throw taskException;
            }

            return returnedObj;
        }

        private static void Wait(ManualResetEvent waitHandler, TimeSpan? timeout)
        {
            if (timeout.HasValue && timeout.Value.TotalSeconds > 0)
            {
                if (!waitHandler.WaitOne(timeout.Value))
                {
                    throw new HttpResponseException(HttpStatusCode.ServiceUnavailable, RestResources.ServiceTimedOut);
                }
            }
            else
            {
                try
                {
                    waitHandler.WaitOne();
                }
                catch (Exception)
                {
                    throw new HttpResponseException(HttpStatusCode.ServiceUnavailable, RestResources.ServiceTimedOut);
                }
            }
        }
    }
}
