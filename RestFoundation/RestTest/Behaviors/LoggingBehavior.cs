using System.Reflection;
using RestFoundation.Behaviors;

namespace RestTest.Behaviors
{
    public class LoggingBehavior : ServiceBehavior
    {
        public override bool OnMethodExecuting(object service, MethodInfo method, object resource)
        {
            Response.SetHttpItem("logging_enabled", true);
            Response.WriteFormat("Action '{0}' executing", method.Name).WriteLine(2);

            return true;
        }

        public override void OnMethodExecuted(object service, MethodInfo method, object result)
        {
            Response.WriteLine(2).WriteFormat("Action '{0}' executed", method.Name);
        }
    }
}
