using System.Diagnostics;
using System.Reflection;
using RestFoundation;
using RestFoundation.Behaviors;

namespace RestTest.Behaviors
{
    public class StatisticsBehavior : ServiceBehavior
    {
        private Stopwatch timer;

        public StatisticsBehavior()
        {
            AffectedMethods = new[] { "Get" };
        }

        public override bool OnMethodExecuting(IServiceContext context, object service, MethodInfo method, object resource)
        {
            timer = Stopwatch.StartNew();

            context.Response.Output.WriteFormat("Service: {0}", service.GetType().Name).WriteLine();
            context.Response.Output.WriteFormat("Method: {0}", method.Name).WriteLine();
            context.Response.Output.WriteFormat("Full URL: {0}", context.Request.Url.ToString()).WriteLine();
            context.Response.Output.WriteFormat("Relative URL: {0}", context.Request.Url.LocalPath).WriteLine();
            context.Response.Output.WriteFormat("Service absolute URL: {0}", context.Request.Url.ServiceUrl.ToString()).WriteLine();
            context.Response.Output.WriteFormat("Service relative URL: {0}", context.Request.Url.ServiceUrl.LocalPath).WriteLine();
            context.Response.Output.WriteFormat("Is Local: {0}", context.Request.IsLocal).WriteLine();
            context.Response.Output.WriteFormat("Is Secure: {0}", context.Request.IsSecure).WriteLine();
            context.Response.Output.WriteFormat("Is AJAX: {0}", context.Request.IsAjax).WriteLine();

            if (context.IsAuthenticated)
            {
                context.Response.Output.WriteFormat("Authenticated as: \"{0}\" ({1})", context.User.Identity.Name, context.User.Identity.AuthenticationType).WriteLine();

                if (context.User.IsInRole("administrator"))
                {
                    context.Response.Output.WriteLine("-- ADMIN ACCESS --");
                }
            }
            else
            {
                context.Response.Output.WriteLine("Not Authenticated");
            }

            context.Response.Output.WriteLine().WriteLine("--- Route Values ---");

            foreach (string key in context.Request.RouteValues.Keys)
            {
                context.Response.Output.WriteFormat("{0} : {1}", key, context.Request.RouteValues.TryGet(key)).WriteLine();
            }

            context.Response.Output.WriteLine().WriteLine("--- Query Values ---");

            foreach (string key in context.Request.QueryString.Keys)
            {
                context.Response.Output.WriteFormat("{0} : {1}", key, context.Request.QueryString.TryGet(key)).WriteLine();
            }

            context.Response.Output.WriteLine();
            return true;
        }

        public override void OnMethodExecuted(IServiceContext context, object service, MethodInfo method, object result)
        {
            timer.Stop();

            context.Response.Output.WriteLine(2).WriteFormat("Response generated in {0} ms ({1} ticks)", timer.ElapsedMilliseconds, timer.ElapsedTicks);
        }
    }
}