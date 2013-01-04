﻿// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Web;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Represents a service method that prevents HTTP caching.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class NoCacheAttribute : ServiceMethodBehaviorAttribute
    {
        /// <summary>
        /// Called after a service method is executed.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="behaviorContext">The "method executed" behavior context.</param>
        public override void OnMethodExecuted(IServiceContext serviceContext, MethodExecutedContext behaviorContext)
        {
            if (serviceContext == null)
            {
                throw new ArgumentNullException("serviceContext");
            }

            HttpContextBase httpContext = serviceContext.GetHttpContext();

            if (httpContext == null)
            {
                throw new ArgumentException(RestResources.MissingHttpContext, "serviceContext");
            }

            httpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            httpContext.Response.Cache.SetValidUntilExpires(false);
            httpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            httpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            httpContext.Response.Cache.SetNoStore();
        }
    }
}