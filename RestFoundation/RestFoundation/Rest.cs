using System;
using System.Globalization;
using System.Web.Routing;
using System.Web.Util;
using RestFoundation.DataFormatters;
using RestFoundation.Runtime;
using RestFoundation.ServiceProxy;

namespace RestFoundation
{
    public sealed class Rest
    {
        internal static readonly Rest Active = new Rest();

        private static readonly object syncRoot = new object();
        private static readonly object formatterSyncRoot = new object();
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

        public IObjectActivator Activator { get; private set; }
        internal bool IsServiceProxyInitialized { get; private set; }

        public Rest UseDataContractSerializers()
        {
            lock (formatterSyncRoot)
            {
                DataFormatterRegistry.SetFormatter("application/json", new DataContractJsonFormatter());
                DataFormatterRegistry.SetFormatter("application/xml", new DataContractXmlFormatter());
                DataFormatterRegistry.SetFormatter("text/xml", new DataContractXmlFormatter());
            }

            return this;
        }

        public Rest WithObjectActivator(IObjectActivator activator)
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

        public Rest WithObjectFactory(Func<Type, object> factory)
        {
            return WithObjectFactory(factory, obj => {});
        }

        public Rest WithObjectFactory(Func<Type, object> factory, Action<object> builder)
        {
            if (factory == null) throw new ArgumentNullException("factory");
            if (builder == null) throw new ArgumentNullException("builder");

            if (Activator != null)
            {
                throw new InvalidOperationException("An object activator or an object factory has already been assigned.");
            }

            Activator = new DelegateObjectActivator(factory, builder);

            MapDefaultUrl();
            return this;
        }

        public Rest WithGlobalBehaviors(Action<GlobalBehaviorBuilder> builder)
        {
            if (builder == null) throw new ArgumentNullException("builder");

            builder(new GlobalBehaviorBuilder());
            return this;
        }

        public Rest WithRoutes(Action<RouteBuilder> builder)
        {
            if (builder == null) throw new ArgumentNullException("builder");

            builder(new RouteBuilder(RouteTable.Routes, Active.CreateObject<IHttpMethodResolver>(), Active.CreateObject<IBrowserDetector>()));
            return this;
        }

        public Rest WithDataFormatters(Action<DataFormatterBuilder> builder)
        {
            if (builder == null) throw new ArgumentNullException("builder");

            builder(new DataFormatterBuilder());
            return this;
        }

        public Rest WithDataBinders(Action<DataBinderBuilder> builder)
        {
            if (builder == null) throw new ArgumentNullException("builder");

            builder(new DataBinderBuilder());
            return this;
        }

        public Rest EnableServiceProxyUI()
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
                throw new ObjectActivationException(String.Format(CultureInfo.InvariantCulture, "Object of type '{0}' could not be activated.", objectType), ex);
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
                throw new ObjectActivationException(String.Format(CultureInfo.InvariantCulture, "Object of type '{0}' could not be activated.", objectType), ex);
            }
        }

        private static void MapDefaultUrl()
        {
            RouteCollection routes = RouteTable.Routes;

            if (routes == null)
            {
                throw new InvalidOperationException("No active routing table was found.");
            }

            if (defaultUrlMapped)
            {
                return;
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
