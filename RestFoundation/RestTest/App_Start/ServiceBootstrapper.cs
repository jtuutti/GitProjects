using System;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using RestFoundation;
using RestFoundation.Behaviors;
using RestFoundation.DataFormatters;
using RestTest.Security;
using RestTest.Behaviors;
using RestTest.ServiceFactories;
using RestTest.StreamCompressors;
using RestTestContracts;
using StructureMap;

namespace RestTest.App_Start
{
    public static class ServiceBootstrapper
    {
        public static void RegisterDependencies()
        {
            // StructureMap IoC container configuration
            ObjectFactory.Configure(ConfigureIoC);

            // Registering RestFoundation.HttpResponseModule using WebActivator for a cleaner Web.config file
            DynamicModuleUtility.RegisterModule(typeof(HttpResponseModule));

            // Configuring REST foundation
            Rest.Configure.WithObjectFactory(CreateObjectFactory)
                          .WithDataFormatters(RegisterDataFormatters)
                          .WithRoutes(RegisterRoutes)
                          .EnableServiceProxyUI();
        }
        private static void ConfigureIoC(ConfigurationExpression config)
        {
            config.Scan(action =>
                        {
                            action.Assembly(ServiceAssembly.Executing);
                            action.WithDefaultConventions();
                        });

            config.ForSingletonOf<IAuthorizationManager>().Use<ServiceAuthorizationManager>();
            config.ForSingletonOf<IStreamCompressor>().Use<RestStreamCompressor>();
            config.For<IServiceFactory>().Use<RestServiceFactory>();

            config.SetAllProperties(convention => convention.TypeMatches(type => type.IsRestDependency()));
        }

        private static object CreateObjectFactory(Type type)
        {
            if (type.IsInterface || type.IsAbstract)
            {
                return ObjectFactory.TryGetInstance(type);
            }

            return ObjectFactory.GetInstance(type);
        }

        private static void RegisterDataFormatters(DataFormatterBuilder builder)
        {
            builder.Set("application/x-www-form-urlencoded", new FormsFormatter());
            builder.Set("multipart/form-data", new MultiPartFormatter());
        }

        private static void RegisterRoutes(RouteBuilder routeBuilder)
        {
            routeBuilder.MapRestRoute<IIndexService>("home")
                        .WithBehaviors(new StatisticsBehavior(), new LoggingBehavior())
                        .DoNotValidateRequests();

            routeBuilder.MapRestRouteAsync<IIndexService>("async")
                        .WithBehaviors(new BasicAuthorizationBehavior(), new StatisticsBehavior(), new LoggingBehavior());

            routeBuilder.MapRestRoute<IDynamicService>("dynamic");

            routeBuilder.MapRestRoute<ITouchMapService>("touch-map")
                        .WithBehaviors(new HttpsOnlyBehavior())
                        .WithContentTypesRestrictedTo("text/xml", "application/xml", "application/json");
        }
    }
}
