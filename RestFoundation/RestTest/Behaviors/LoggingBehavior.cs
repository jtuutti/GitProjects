using RestFoundation;
using RestFoundation.Behaviors;

namespace RestTest.Behaviors
{
    public class LoggingBehavior : ServiceBehavior
    {
        public override BehaviorMethodAction OnMethodExecuting(IServiceContext serviceContext, MethodExecutingContext behaviorContext)
        {
            serviceContext.Request.ResourceBag.LoggingEnabled = true;
            serviceContext.Response.Output.WriteFormat("Action '{0}' executing", behaviorContext.GetMethodName()).WriteLine(2);

            return BehaviorMethodAction.Execute;
        }

        public override void OnMethodExecuted(IServiceContext serviceContext, MethodExecutedContext behaviorContext)
        {
            serviceContext.Response.Output.WriteLine(2).WriteFormat("Action '{0}' executed", behaviorContext.GetMethodName());
        }
    }
}
