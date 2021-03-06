﻿using System.Diagnostics;
using System.Linq;
using RestFoundation;
using RestFoundation.Behaviors;

namespace RestTestContracts.Behaviors
{
    /// <summary>
    /// Represents a service behavior responsible for service statistics.
    /// </summary>
    public class StatisticsBehavior : ServiceBehavior
    {
        private Stopwatch timer;

        /// <summary>
        /// Returns a value indicating whether to apply the behavior to the provided method of the specified
        /// service type.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="methodContext">The "method applies" context.</param>
        /// <returns>true to apply the behavior; false to bypass.</returns>
        public override bool AppliesTo(IServiceContext serviceContext, MethodAppliesContext methodContext)
        {
            return methodContext.GetSupportedHttpMethods().Contains(HttpMethod.Get);
        }

        /// <summary>
        /// Called before a service method is executed.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="behaviorContext">The "method executing" behavior context.</param>
        /// <returns>A service method action.</returns>
        public override BehaviorMethodAction OnMethodExecuting(IServiceContext serviceContext, MethodExecutingContext behaviorContext)
        {
            timer = Stopwatch.StartNew();

            serviceContext.Response.Output.WriteFormat("Contract: {0}", behaviorContext.GetServiceContractType().Name).WriteLine();
            serviceContext.Response.Output.WriteFormat("Service: {0}", behaviorContext.GetServiceType().Name).WriteLine();
            serviceContext.Response.Output.WriteFormat("Method: {0}", behaviorContext.GetMethodName()).WriteLine();
            serviceContext.Response.Output.WriteFormat("Full URL: {0}", serviceContext.Request.Url.ToString()).WriteLine();
            serviceContext.Response.Output.WriteFormat("Relative URL: {0}", serviceContext.Request.Url.LocalPath).WriteLine();
            serviceContext.Response.Output.WriteFormat("Service absolute URL: {0}", serviceContext.Request.Url.OperationUrl.ToString()).WriteLine();
            serviceContext.Response.Output.WriteFormat("Service relative URL: {0}", serviceContext.Request.Url.OperationUrl.LocalPath).WriteLine();
            serviceContext.Response.Output.WriteFormat("Is Local: {0}", serviceContext.Request.IsLocal).WriteLine();
            serviceContext.Response.Output.WriteFormat("Is Secure: {0}", serviceContext.Request.IsSecure).WriteLine();
            serviceContext.Response.Output.WriteFormat("Is AJAX: {0}", serviceContext.Request.IsAjax).WriteLine();

            if (serviceContext.IsAuthenticated)
            {
                serviceContext.Response.Output.WriteFormat("Authenticated as: \"{0}\" ({1})", serviceContext.User.Identity.Name, serviceContext.User.Identity.AuthenticationType).WriteLine();

                if (serviceContext.User.IsInRole("Administrators"))
                {
                    serviceContext.Response.Output.WriteLine("-- ADMIN ACCESS --");
                }
            }
            else
            {
                serviceContext.Response.Output.WriteLine("Not Authenticated");
            }

            serviceContext.Response.Output.WriteLine().WriteLine("--- Route Values ---");

            foreach (string key in serviceContext.Request.RouteValues.Keys)
            {
                serviceContext.Response.Output.WriteFormat("{0} : {1}", key, serviceContext.Request.RouteValues.TryGet(key)).WriteLine();
            }

            serviceContext.Response.Output.WriteLine().WriteLine("--- Query Values ---");

            foreach (string key in serviceContext.Request.QueryString.Keys)
            {
                serviceContext.Response.Output.WriteFormat("{0} : {1}", key, serviceContext.Request.QueryString.TryGet(key)).WriteLine();
            }

            serviceContext.Response.Output.WriteLine();

            return BehaviorMethodAction.Execute;
        }

        /// <summary>
        /// Called after a service method is executed.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="behaviorContext">The "method executed" behavior context.</param>
        public override void OnMethodExecuted(IServiceContext serviceContext, MethodExecutedContext behaviorContext)
        {
            timer.Stop();

            serviceContext.Response.Output.WriteLine(2).WriteFormat("Response generated in {0} ms ({1} ticks)", timer.ElapsedMilliseconds, timer.ElapsedTicks);
        }
    }
}