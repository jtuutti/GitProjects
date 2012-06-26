﻿using System;
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
    }
}