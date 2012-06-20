using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using RestFoundation;
using RestFoundation.Behaviors;
using RestTest.Behaviors;
using RestTest.ServiceFactories;
using RestTestContracts;
using StructureMap;

namespace RestTest.App_Start
{
    public static class ServiceBootstrapper
    {
        private static readonly string[] standardContentTypes = new[]
                                                                {
                                                                    "application/json",
                                                                    "application/xml",
                                                                    "text/xml"
                                                                };

        private static readonly string[] homeContentTypes = new[]
                                                            {
                                                                "application/json",
                                                                "application/xml",
                                                                "text/xml",
                                                                "application/x-www-form-urlencoded"
                                                            };

        public static void RegisterDependencies()
        {
            DynamicModuleUtility.RegisterModule(typeof(HttpResponseModule));

            ObjectFactory.Configure(config =>
                                    {
                                        config.Scan(action =>
                                                    {
                                                        action.Assembly(ServiceAssembly.Executing);
                                                        action.WithDefaultConventions();
                                                    });

                                        config.For<IServiceFactory>().Use<RestServiceFactory>();
                                        config.SetAllProperties(convention => convention.TypeMatches(type => type == typeof(IServiceContext) ||
                                                                                                             type == typeof(IHttpRequest) ||
                                                                                                             type == typeof(IHttpResponse)));
                                    });

            ServiceLocator.SetLocatorProvider(() => new StructureMapServiceLocator(ObjectFactory.Container));

            RegisterRoutes();
        }

        private static void RegisterRoutes()
        {           
            RouteTable.Routes.AddGlobalBehaviors(new ContentTypeBehavior(standardContentTypes));

            RouteTable.Routes.MapRestRoute<IIndexService>("home").WithBehaviors(new StatisticsBehavior(), new ContentTypeBehavior(homeContentTypes), new LoggingBehavior()).DoNotValidateRequests();
            RouteTable.Routes.MapRestRouteAsync<IIndexService>("async").WithBehaviors(new StatisticsBehavior(), new ContentTypeBehavior(homeContentTypes), new LoggingBehavior());
            RouteTable.Routes.MapRestRoute<ITouchMapService>("touch-map");
        }

        #region ServiceLocator

        private sealed class StructureMapServiceLocator : IServiceLocator
        {
            private const string ActivationExceptionMessage = "There was an exception locating service of type '{0}'";

            private readonly IContainer container;

            public StructureMapServiceLocator(IContainer container)
            {
                if (container == null)
                    throw new ArgumentNullException("container");

                this.container = container;
            }

            public object GetInstance(Type serviceType)
            {
                return GetInstance(serviceType, null);
            }

            public object GetInstance(Type serviceType, string key)
            {
                if (serviceType == null)
                    throw new ArgumentNullException("serviceType");

                try
                {
                    if (serviceType.IsAbstract || serviceType.IsInterface)
                    {
                        return key != null ? container.TryGetInstance(serviceType, key) : container.TryGetInstance(serviceType);
                    }

                    return key != null ? container.GetInstance(serviceType, key) : container.GetInstance(serviceType);
                }
                catch (Exception ex)
                {
                    throw new ActivationException(String.Format(ActivationExceptionMessage, serviceType.FullName), ex);
                }
            }

            public IEnumerable<object> GetAllInstances(Type serviceType)
            {
                if (serviceType == null)
                    throw new ArgumentNullException("serviceType");

                try
                {
                    return container.GetAllInstances(serviceType).Cast<object>();
                }
                catch (Exception ex)
                {
                    throw new ActivationException(String.Format(ActivationExceptionMessage, serviceType.FullName), ex);
                }
            }

            public TService GetInstance<TService>()
            {
                return GetInstance<TService>(null);
            }

            public TService GetInstance<TService>(string key)
            {
                Type serviceType = typeof(TService);

                try
                {
                    if (serviceType.IsAbstract || serviceType.IsInterface)
                    {
                        return key != null ? container.TryGetInstance<TService>(key) : container.TryGetInstance<TService>();
                    }

                    return key != null ? container.GetInstance<TService>(key) : container.GetInstance<TService>();
                }
                catch (Exception ex)
                {
                    throw new ActivationException(String.Format(ActivationExceptionMessage, serviceType.FullName), ex);
                }
            }

            public IEnumerable<TService> GetAllInstances<TService>()
            {
                try
                {
                    return container.GetAllInstances<TService>();
                }
                catch (Exception ex)
                {
                    throw new ActivationException(String.Format(ActivationExceptionMessage, typeof(TService).FullName), ex);
                }
            }

            object IServiceProvider.GetService(Type serviceType)
            {
                return GetInstance(serviceType, null);
            }
        }

        #endregion
    }
}
