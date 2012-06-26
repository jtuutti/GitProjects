using System.Diagnostics;
using System.Reflection;
using RestFoundation.Behaviors;

namespace RestTest.Behaviors
{
    public class StatisticsBehavior : ServiceBehavior
    {
        private Stopwatch timer;

        public override bool OnMethodExecuting(object service, MethodInfo method, object resource)
        {
            timer = Stopwatch.StartNew();

            Response.Output.WriteFormat("Service: {0}", service.GetType().Name).WriteLine();
            Response.Output.WriteFormat("Method: {0}", method.Name).WriteLine();
            Response.Output.WriteFormat("Full URL: {0}", Request.Url.ToString()).WriteLine();
            Response.Output.WriteFormat("Relative URL: {0}", Request.Url.LocalPath).WriteLine();
            Response.Output.WriteFormat("Service absolute URL: {0}", Request.Url.ServiceUrl.ToString()).WriteLine();
            Response.Output.WriteFormat("Service relative URL: {0}", Request.Url.ServiceUrl.LocalPath).WriteLine();
            Response.Output.WriteFormat("Is Authenticated: {0}", Context.IsAuthenticated).WriteLine();
            Response.Output.WriteFormat("Is Local: {0}", Request.IsLocal).WriteLine();
            Response.Output.WriteFormat("Is Secure: {0}", Request.IsSecure).WriteLine();
            Response.Output.WriteFormat("Is AJAX: {0}", Request.IsAjax).WriteLine();

            Response.Output.WriteLine().WriteLine("--- Route Values ---");

            foreach (string key in Request.RouteValues.Keys)
            {
                Response.Output.WriteFormat("{0} : {1}", key, Request.RouteValues.TryGet(key)).WriteLine();
            }

            Response.Output.WriteLine().WriteLine("--- Query Values ---");

            foreach (string key in Request.QueryString.Keys)
            {
                Response.Output.WriteFormat("{0} : {1}", key, Request.QueryString.TryGet(key)).WriteLine();
            }

            Response.Output.WriteLine();
            return true;
        }

        public override void OnMethodExecuted(object service, MethodInfo method, object result)
        {
            timer.Stop();

            Response.Output.WriteLine(2).WriteFormat("Response generated in {0} ms ({1} ticks)", timer.ElapsedMilliseconds, timer.ElapsedTicks);
        }
    }
}