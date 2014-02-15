using System;
using RestFoundation;
using RestFoundation.Behaviors;
using RestTestContracts.Resources;

namespace RestTestContracts.Behaviors
{
    /// <summary>
    /// Represents a service behavior attribute responsible for parsing custom SmartCare headers.
    /// </summary>
    public sealed class T3ContextBehaviorAttribute : ServiceMethodBehaviorAttribute
    {
        /// <summary>
        /// Gets the status description if no valid session was found.
        /// </summary>
        public override string StatusDescription
        {
            get
            {
                return "No valid session context found";
            }
        }

        /// <summary>
        /// Called during the authorization process before a service method or behavior is executed.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="behaviorContext">The "method authorizing" behavior context.</param>
        /// <returns>A service method action.</returns>
        public override BehaviorMethodAction OnMethodAuthorizing(IServiceContext serviceContext, MethodAuthorizingContext behaviorContext)
        {
            var sessionInfo = new SessionInfo(serviceContext.Request.Headers.TryGet("X-SpeechCycle-SmartCare-ApplicationID"),
                                              serviceContext.Request.Headers.TryGet("X-SpeechCycle-SmartCare-CustomerID"),
                                              serviceContext.Request.Headers.TryGet("X-SpeechCycle-SmartCare-SessionID"),
                                              serviceContext.Request.Headers.TryGet("X-SpeechCycle-SmartCare-CultureCode"),
                                              serviceContext.Request.Headers.TryGet("X-SpeechCycle-SmartCare-Environment"));

            if (String.IsNullOrEmpty(sessionInfo.ApplicationId) || String.IsNullOrEmpty(sessionInfo.CustomerId) ||
                String.IsNullOrEmpty(sessionInfo.Environment) || sessionInfo.SessionId == Guid.Empty)
            {
                return BehaviorMethodAction.Stop;
            }

            serviceContext.Request.ResourceBag.SessionInfo = sessionInfo;
            return BehaviorMethodAction.Execute;
        }
    }
}
