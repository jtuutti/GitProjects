using RestFoundation;
using RestFoundation.Behaviors;

namespace RestTestContracts.Behaviors
{
    /// <summary>
    /// Represents a service behavior responsible for logging.
    /// </summary>
    public class LoggingBehavior : ServiceBehavior
    {
        /// <summary>
        /// Called before a service method is executed.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="behaviorContext">The "method executing" behavior context.</param>
        /// <returns>A service method action.</returns>
        public override BehaviorMethodAction OnMethodExecuting(IServiceContext serviceContext, MethodExecutingContext behaviorContext)
        {
            serviceContext.Request.ResourceBag.LoggingEnabled = true;
            serviceContext.Response.Output.WriteFormat("Action '{0}' executing", behaviorContext.GetMethodName()).WriteLine(2);

            return BehaviorMethodAction.Execute;
        }

        /// <summary>
        /// Called after a service method is executed.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="behaviorContext">The "method executed" behavior context.</param>
        public override void OnMethodExecuted(IServiceContext serviceContext, MethodExecutedContext behaviorContext)
        {
            serviceContext.Response.Output.WriteLine(2).WriteFormat("Action '{0}' executed", behaviorContext.GetMethodName());
        }
    }
}
