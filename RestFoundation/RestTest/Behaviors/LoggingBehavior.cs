using System.Reflection;
using RestFoundation.Behaviors;

namespace RestTest.Behaviors
{
    public class LoggingBehavior : ServiceBehavior
    {
        private string methodName;

        public override void OnActionBinding(object service, MethodInfo actionMethod)
        {
            methodName = actionMethod.Name;

            Response.SetHttpItem("logging_enabled", true);
        }

        public override bool OnActionExecuting(object resource)
        {
            Response.WriteFormat("Action '{0}' executing", methodName).WriteLine(2);
            return true;
        }

        public override void OnActionExecuted(object result)
        {
            Response.WriteLine(2).WriteFormat("Action '{0}' executed", methodName);
        }
    }
}
