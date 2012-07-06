using System;
using System.Reflection;
using RestFoundation.Behaviors;
using RestTestContracts.Resources;

namespace RestTestServices.Behaviors
{
    public class T3ContextBehavior : ServiceSecurityBehavior
    {
        public override bool OnMethodAuthorizing(RestFoundation.IServiceContext context, object service, MethodInfo method)
        {
            var sessionInfo = new SessionInfo(context.Request.Headers.TryGet("X-SpeechCycle-SmartCare-ApplicationID"),
                                              context.Request.Headers.TryGet("X-SpeechCycle-SmartCare-CustomerID"),
                                              context.Request.Headers.TryGet("X-SpeechCycle-SmartCare-SessionID"),
                                              context.Request.Headers.TryGet("X-SpeechCycle-SmartCare-CultureCode"),
                                              context.Request.Headers.TryGet("X-SpeechCycle-SmartCare-Environment"));

            if (String.IsNullOrEmpty(sessionInfo.ApplicationId) || String.IsNullOrEmpty(sessionInfo.CustomerId) ||
                String.IsNullOrEmpty(sessionInfo.Environment) || sessionInfo.SessionId == Guid.Empty)
            {
                SetForbiddenErrorMessage("No valid session context found");
                return false;
            }

            context.ItemBag.SessionInfo = sessionInfo;
            return true;
        }
    }
}
