// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Reflection;
using System.Web;

namespace RestFoundation.Runtime
{
    internal static class ExceptionUnwrapper
    {
        public static bool IsDirectResponseException(Exception ex)
        {
            return ex is HttpResponseException || ex is HttpResourceFaultException || ex is HttpRequestValidationException;
        }

        public static Exception Unwrap(Exception ex)
        {
            if (ex == null)
            {
                return null;
            }

            return IsWrapperException(ex) ? Unwrap(ex.InnerException) : ex;
        }

        private static bool IsWrapperException(Exception ex)
        {
            return ex.InnerException != null && (ex is ServiceRuntimeException || ex is TargetInvocationException || ex is AggregateException);
        }
    }
}
