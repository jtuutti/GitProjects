using System.Collections.Generic;
using System.Reflection;
using MvcAlt.Binders;
using MvcAlt.Infrastructure;

namespace MvcAlt
{
    public class MvcConfiguration
    {
        protected MvcConfiguration()
        {
            ActionMethodResolver = new DefaultActionMethodResolver();
            ModelFactory = new DefaultModelFactory();

            BindersByType.Add("*", new IBinder[]
            {
                new RouteBinder(),
                new QueryBinder()
            });

            BindersByType.Add("application/x-www-form-urlencoded", new IBinder[]
            {
                new RouteBinder(),
                new QueryBinder(),
                new FormBinder()
            });
        }

        public static MvcConfiguration Current { get; protected set; }

        public readonly Dictionary<string, IBinder[]> BindersByType = new Dictionary<string, IBinder[]>();

        public IActionMethodResolver ActionMethodResolver { get; protected set; }
        public IModelFactory ModelFactory { get; protected set; }
        public Assembly ControllerAssembly { get; set; }

        static MvcConfiguration()
        {
            Current = new MvcConfiguration
            {
                ControllerAssembly = Assembly.Load("MvcAlt")
            };
        }
    }
}
