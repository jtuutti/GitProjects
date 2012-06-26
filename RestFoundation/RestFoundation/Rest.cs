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
        private IObjectActivator m_activator;

        protected Rest()
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
                    routes.Add(new Route(String.Empty, new RootRouteHandler()));
                    defaultUrlMapped = true;
                }
            }
        }

        protected internal bool IsServiceProxyInitialized { get; protected set; }

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

        public virtual Rest WithObjectActivator(IObjectActivator activator)
        {
            if (activator == null) throw new ArgumentNullException("activator");

            if (m_activator != null)
            {
                throw new InvalidOperationException("An object activator or an object factory has already been assigned.");
            }

            m_activator = activator;
            return this;
        }

        public virtual Rest WithObjectFactory(Func<Type, object> factory)
        {
            if (factory == null) throw new ArgumentNullException("factory");

            if (m_activator != null)
            {
                throw new InvalidOperationException("An object activator or an object factory has already been assigned.");
            }

            m_activator = new DelegateObjectActivator(factory);
            return this;
        }

        public virtual Rest WithGlobalBehaviors(Action<BehaviorBuilder> builder)
        {
            builder(new BehaviorBuilder());
            return this;
        }

        public virtual Rest WithRoutes(Action<RouteBuilder> builder)
        {
            builder(new RouteBuilder(RouteTable.Routes));
            return this;
        }

        public virtual Rest WithDataFormatters(Action<DataFormatterBuilder> builder)
        {
            builder(new DataFormatterBuilder());
            return this;
        }

        public virtual Rest EnableServiceProxyUI()
        {
            if (IsServiceProxyInitialized)
            {
                throw new InvalidOperationException("Service proxy UI is already enabled.");
            }

            ProxyPathProvider.AppInitialize();
            IsServiceProxyInitialized = true;

            return this;
        }

        internal object CreateObject(Type objectType)
        {
            if (objectType == null) throw new ArgumentNullException("objectType");

            if (m_activator == null)
            {
                throw new ObjectActivationException("No object activator or factory has been assigned.");
            }

            try
            {
                return m_activator.Create(objectType);
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
    }
}
