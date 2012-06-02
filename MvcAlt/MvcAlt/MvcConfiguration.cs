using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;
using MvcAlt.Binders;
using MvcAlt.Infrastructure;

namespace MvcAlt
{
    public class MvcConfiguration
    {
        protected MvcConfiguration()
        {
            ActionMethodResolver = new DefaultActionMethodResolver();

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
        public Assembly ControllerAssembly { get; set; }

        internal IServiceLocator MvcServiceLocator
        {
            get
            {
                try
                {
                    if (ServiceLocator.Current == null)
                    {
                        throw new Exception();
                    }
                }
                catch (Exception)
                {
                    throw new ActivationException("No service locator is available. You can set it by calling the " +
                                                  "'Microsoft.Practices.ServiceLocation.ServiceLocator.SetLocatorProvider(newProvider)' function " +
                                                  "from the 'Application_Start' method in the 'Global.asax' file.");
                }

                return ServiceLocator.Current;
            }
        }

        static MvcConfiguration()
        {
            Current = new MvcConfiguration
            {
                ControllerAssembly = Assembly.Load("MvcAlt")
            };
        }
    }
}
