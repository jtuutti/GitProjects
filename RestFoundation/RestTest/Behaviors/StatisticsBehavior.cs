using System.Diagnostics;
using System.Reflection;
using RestFoundation.Behaviors;

namespace RestTest.Behaviors
{
    public class StatisticsBehavior : ServiceBehavior
    {
        private Stopwatch timer;

        public override void OnActionBinding(object service, MethodInfo actionMethod)
        {
            timer = Stopwatch.StartNew();

            Response.WriteFormat("Service: {0}", service.GetType().Name).WriteLine();
            Response.WriteFormat("Method: {0}", actionMethod.Name).WriteLine();
            Response.WriteFormat("Full URL: {0}", Request.Url.ToString()).WriteLine();
            Response.WriteFormat("Relative URL: {0}", Request.Url.LocalPath).WriteLine();
            Response.WriteFormat("Service absolute URL: {0}", Request.Url.ServiceUrl.ToString()).WriteLine();
            Response.WriteFormat("Service relative URL: {0}", Request.Url.ServiceUrl.LocalPath).WriteLine();
            Response.WriteFormat("Is Local: {0}", Request.IsLocal).WriteLine();
            Response.WriteFormat("Is Secure: {0}", Request.IsSecure).WriteLine();
            Response.WriteFormat("Is AJAX: {0}", Request.IsAjax).WriteLine();

            Response.WriteLine().WriteLine("--- Route Values ---");

            foreach (string key in Request.RouteValues.Keys)
            {
                Response.WriteFormat("{0} : {1}", key, Request.RouteValues.TryGet(key)).WriteLine();
            }

            Response.WriteLine().WriteLine("--- Query Values ---");

            foreach (string key in Request.QueryString.Keys)
            {
                Response.WriteFormat("{0} : {1}", key, Request.QueryString.TryGet(key)).WriteLine();
            }

            Response.WriteLine();
        }

        public override void OnActionExecuted(object result)
        {
            timer.Stop();

            Response.WriteLine(2).WriteFormat("Response generated in {0} ms ({1} ticks)", timer.ElapsedMilliseconds, timer.ElapsedTicks);
        }
    }
}