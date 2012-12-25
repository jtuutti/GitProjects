using System;
using RestFoundation;
using RestFoundation.Behaviors;
using RestTestContracts.Resources;

namespace RestTestContracts.Behaviors
{
    public sealed class T3ContextBehaviorAttribute : ServiceMethodBehaviorAttribute
    {
        public override string StatusDescription
        {
            get
            {
                return "No valid session context found";
            }
        }

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
