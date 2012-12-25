// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Globalization;
using System.Web;

namespace RestFoundation.Runtime
{
    internal static class LogUtility
    {
        private const string DefaultContentType = "text/html";

        private static readonly object syncRoot = new object();
        private static ILogWriter writer;

        public static bool CanLog
        {
            get
            {
                ILogWriter writerSnapshot = Writer;

                if (writerSnapshot == null || !writerSnapshot.LogGeneratedInfo)
                {
                    return false;
                }

                var serviceContext = Rest.Configuration.ServiceLocator.GetService<IServiceContext>();

                return serviceContext.Request.ResourceBag.ServiceExecutionId != null;
            }
        }

        public static ILogWriter Writer
        {
            get
            {
                if (writer == null)
                {
                    writer = Rest.Configuration.ServiceLocator.GetService<ILogWriter>();
                }

                if (writer == null)
                {
                    throw new InvalidOperationException(RestResources.UnregisteredLogWriter);
                }

                return writer;
            }
        }

        public static void LogRequestData(HttpContextBase httpContext)
        {
            if (!CanLog)
            {
                return;
            }

            lock (syncRoot)
            {
                if (CanLog)
                {
                    Writer.WriteInfo(RestResources.ServiceCallStarted);

                    if (httpContext == null || httpContext.Request == null || httpContext.Response == null)
                    {
                        return;
                    }

                    Writer.WriteInfo(String.Empty)
                          .WriteInfo(String.Format(CultureInfo.InvariantCulture, "REQUEST GUID: {0}", httpContext.Items["ServiceExecutionId"] ?? "N/A"))
                          .WriteInfo(String.Empty)
                          .WriteInfo(String.Format(CultureInfo.InvariantCulture, "URL: {0}", httpContext.Request.RawUrl))
                          .WriteInfo(String.Format(CultureInfo.InvariantCulture, "IS CONNECTION SECURE: {0}", httpContext.Request.IsSecureConnection.ToString().ToLowerInvariant()))
                          .WriteInfo(String.Format(CultureInfo.InvariantCulture, "IS REQUEST LOCAL: {0}", httpContext.Request.IsLocal.ToString().ToLowerInvariant()))
                          .WriteInfo(String.Format(CultureInfo.InvariantCulture, "HTTP METHOD: {0}", httpContext.Request.HttpMethod))
                          .WriteInfo(String.Format(CultureInfo.InvariantCulture, "USER HOST: {0}", httpContext.Request.UserHostName))
                          .WriteInfo(String.Format(CultureInfo.InvariantCulture, "USER IP: {0}", httpContext.Request.UserHostAddress))
                          .WriteInfo(String.Format(CultureInfo.InvariantCulture, "USER AGENT: {0}", httpContext.Request.UserAgent));

                    Writer.WriteInfo(String.Empty).WriteInfo("REQUEST HEADERS");

                    foreach (string headerName in httpContext.Request.Headers.AllKeys)
                    {
                        Writer.WriteInfo(String.Format(CultureInfo.InvariantCulture, "{0} : {1}", headerName, httpContext.Request.Headers.Get(headerName)));
                    }
                }
            }
        }

        public static void LogResponseData(HttpContextBase httpContext)
        {
            if (!CanLog)
            {
                return;
            }

            lock (syncRoot)
            {
                if (CanLog)
                {
                    if (httpContext == null || httpContext.Response == null || httpContext.Response.Headers.AllKeys.Length == 0)
                    {
                        return;
                    }

                    Writer.WriteInfo(String.Empty).WriteInfo("RESPONSE HEADERS:");

                    foreach (string headerName in httpContext.Response.Headers.AllKeys)
                    {
                        Writer.WriteInfo(String.Format(CultureInfo.InvariantCulture, "{0} : {1}", headerName, httpContext.Response.Headers.Get(headerName)));
                    }

                    Writer.WriteInfo(String.Empty).WriteInfo(String.Concat("RESPONSE STATUS: ", httpContext.Response.Status.Replace(' ', '/')));
                }
            }
        }

        public static void LogRequestBody(string body, string contentType)
        {
            if (!CanLog)
            {
                return;
            }

            lock (syncRoot)
            {
                if (CanLog && !String.IsNullOrEmpty(body))
                {
                    if (String.IsNullOrEmpty(contentType))
                    {
                        contentType = DefaultContentType;
                    }

                    Writer.WriteInfo(String.Empty)
                          .WriteInfo(String.Format(CultureInfo.InvariantCulture, "REQUEST BODY [{0} - {1}]:", contentType, body.Length))
                          .WriteInfo(body);
                }
            }
        }

        public static void LogResponseBody(string body, string contentType)
        {
            if (!CanLog)
            {
                return;
            }

            lock (syncRoot)
            {
                if (CanLog && !String.IsNullOrEmpty(body))
                {
                    if (String.IsNullOrEmpty(contentType))
                    {
                        contentType = DefaultContentType;
                    }

                    Writer.WriteInfo(String.Empty)
                          .WriteInfo(String.Format(CultureInfo.InvariantCulture, "RESPONSE BODY [{0} - {1}]:", contentType, body.Length))
                          .WriteInfo(body);
                }
            }
        }

        public static void LogEndResponse()
        {
            if (!CanLog)
            {
                return;
            }

            lock (syncRoot)
            {
                if (CanLog)
                {
                    Writer.WriteInfo(String.Empty).WriteInfo(RestResources.ServiceCallEnded);
                }
            }
        }

        public static void LogUnsealedBehaviorAttribute(Type behaviorAttributeType)
        {
            if (!CanLog)
            {
                return;
            }

            lock (syncRoot)
            {
                if (CanLog)
                {
                    Writer.WriteWarning(String.Format(CultureInfo.InvariantCulture, RestResources.UnsealedBehaviorAttributeClass, behaviorAttributeType.FullName));
                }
            }
        }
    }
}
