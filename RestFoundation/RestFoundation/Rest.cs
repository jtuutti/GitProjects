using System;
using System.Web.Routing;
using System.Web.Util;
using RestFoundation.Runtime;
using RestFoundation.ServiceProxy;

namespace RestFoundation
{
    public class Rest
    {
        protected static readonly object syncRoot = new object();
        protected internal static Rest Active = new Rest();

        private static bool defaultUrlMapped;

        public static Rest Configure
        {
            get
            {
                if (!(RequestValidator.Current is ServiceRequestValidator))
                {
                    RequestValidator.Current = new ServiceRequestValidator();
                }

                return Active;
            }
        }

        public IObjectActivator Activator { get; protected set; }
        protected internal bool IsServiceProxyInitialized { get; protected set; }

        public virtual Rest WithObjectActivator(IObjectActivator activator)
        {
            if (activator == null) throw new ArgumentNullException("activator");

            if (Activator != null)
            {
                throw new InvalidOperationException("An object activator or an object factory has already been assigned.");
            }

            Activator = activator;

            MapDefaultUrl();
            return this;
        }

        public virtual Rest WithObjectFactory(Func<Type, object> factory)
        {
            if (factory == null) throw new ArgumentNullException("factory");

            if (Activator != null)
            {
                throw new InvalidOperationException("An object activator or an object factory has already been assigned.");
            }

            Activator = new DelegateObjectActivator(factory);

            MapDefaultUrl();
            return this;
        }

        public virtual Rest WithGlobalBehaviors(Action<GlobalBehaviorBuilder> builder)
        {
            builder(new GlobalBehaviorBuilder());
            return this;
        }

        public virtual Rest WithRoutes(Action<RouteBuilder> builder)
        {
            builder(new RouteBuilder(RouteTable.Routes, Active.CreateObject<IHttpMethodResolver>()));
            return this;
        }

        public virtual Rest WithDataFormatters(Action<DataFormatterBuilder> builder)
        {
            builder(new DataFormatterBuilder());
            return this;
        }

        public virtual Rest WithDataBinders(Action<DataBinderBuilder> builder)
        {
            builder(new DataBinderBuilder());
            return this;
        }

        public virtual Rest EnableServiceProxyUI()
        {
            if (IsServiceProxyInitialized)
            {
                throw new InvalidOperationException("Service proxy UI is already enabled.");
            }

            ProxyPathProvider.AppInitialize();

            RouteTable.Routes.MapPageRoute("ProxyIndex", "help/index", "~/index.aspx");
            RouteTable.Routes.MapPageRoute(String.Empty, "help/metadata", "~/metadata.aspx");
            RouteTable.Routes.MapPageRoute(String.Empty, "help/output", "~/output.aspx");
            RouteTable.Routes.MapPageRoute(String.Empty, "help/proxy", "~/proxy.aspx");
            RouteTable.Routes.Add(new Route("help", new ProxyRootHandler()));

            IsServiceProxyInitialized = true;

            return this;
        }

        internal object CreateObject(Type objectType)
        {
            if (objectType == null) throw new ArgumentNullException("objectType");

            if (Activator == null)
            {
                throw new ObjectActivationException("No object activator or factory has been assigned.");
            }

            try
            {
                return Activator.Create(objectType);
            }
            catch (Exception ex)
            {
                throw new ObjectActivationException(String.Format("Object of type '{0}' could not be activated.", objectType), ex);
            }
        }

        internal T CreateObject<T>()
        {
            Type objectType = typeof(T);

            try
            {
                return (T) CreateObject(objectType);
            }
            catch (Exception ex)
            {
                throw new ObjectActivationException(String.Format("Object of type '{0}' could not be activated.", objectType), ex);
            }
        }

        protected void MapDefaultUrl()
        {
            if (defaultUrlMapped)
            {
                return;
            }

            RouteCollection routes = RouteTable.Routes;

            if (routes == null)
            {
                throw new InvalidOperationException("No active routing table was found.");
            }

            lock (syncRoot)
            {
                if (!defaultUrlMapped)
                {
                    routes.Add(new Route(String.Empty, Active.CreateObject<RootRouteHandler>()));
                    defaultUrlMapped = true;
                }
            }
        }
    }
}
