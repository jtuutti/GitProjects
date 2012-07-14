using System.Reflection;
using RestFoundation;
using RestFoundation.Behaviors;

namespace RestTest.Behaviors
{
    public class LoggingBehavior : ServiceBehavior
    {
        public override bool OnMethodExecuting(IServiceContext context, object service, MethodInfo method, object resource)
        {
            context.ItemBag.LoggingEnabled = true;
            context.Response.Output.WriteFormat("Action '{0}' executing", method.Name).WriteLine(2);

            return true;
        }

        public override void OnMethodExecuted(IServiceContext context, object service, MethodInfo method, object returnedObj)
        {
            context.Response.Output.WriteLine(2).WriteFormat("Action '{0}' executed", method.Name);
        }
    }
}
