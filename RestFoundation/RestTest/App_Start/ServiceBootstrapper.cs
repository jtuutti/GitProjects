using System;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using RestFoundation;
using RestFoundation.DataFormatters;
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
                          .WithDataFormatters(builder => builder.SetForContentType("application/x-www-form-urlencoded", new FormsFormatter()))
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

            config.For<IServiceFactory>().Use<RestServiceFactory>();
            config.ForSingletonOf<IStreamCompressor>().Use<RestStreamCompressor>();

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

        private static void RegisterRoutes(RouteBuilder routeBuilder)
        {
            routeBuilder.MapRestRoute<IIndexService>("home")
                        .WithBehaviors(new StatisticsBehavior(), new LoggingBehavior())
                        .DoNotValidateRequests();

            routeBuilder.MapRestRouteAsync<IIndexService>("async")
                        .WithBehaviors(new StatisticsBehavior(), new LoggingBehavior());

            routeBuilder.MapRestRoute<IDynamicService>("dynamic");

            routeBuilder.MapRestRoute<ITouchMapService>("touch-map")
                        .WithContentTypesRestrictedTo("text/xml", "application/xml", "application/json");
        }
    }
}
