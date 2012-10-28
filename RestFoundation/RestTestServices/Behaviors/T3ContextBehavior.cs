using System;
using RestFoundation;
using RestFoundation.Behaviors;
using RestTestContracts.Resources;

namespace RestTestServices.Behaviors
{
    public class T3ContextBehavior : SecureServiceBehavior
    {
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
                SetStatusDescription("No valid session context found");
                return BehaviorMethodAction.Stop;
            }

            serviceContext.HttpItemBag.SessionInfo = sessionInfo;
            return BehaviorMethodAction.Execute;
        }
    }
}
