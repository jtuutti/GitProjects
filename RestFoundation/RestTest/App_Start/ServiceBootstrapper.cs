using System;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using RestFoundation;
using RestFoundation.Behaviors;
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
            // StructureMap IoC container configuration
            ObjectFactory.Configure(ConfigureIoC);

            // Registering RestFoundation.HttpResponseModule using WebActivator for a cleaner Web.config file
            DynamicModuleUtility.RegisterModule(typeof(HttpResponseModule));

            // Configuring REST foundation
            Rest.Configure.WithObjectFactory(CreateObjectFactory)
                          .WithDataFormatters(builder => builder.SetForContentType("application/x-www-form-urlencoded", new FormsFormatter()))
                          .WithGlobalBehaviors(builder => builder.AddGlobalBehaviors(new ContentTypeBehavior(standardContentTypes)))
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
                        .WithBehaviors(new StatisticsBehavior(), new ContentTypeBehavior(homeContentTypes), new LoggingBehavior())
                        .DoNotValidateRequests();

            routeBuilder.MapRestRouteAsync<IIndexService>("async")
                        .WithBehaviors(new StatisticsBehavior(), new ContentTypeBehavior(homeContentTypes), new LoggingBehavior());

            routeBuilder.MapRestRoute<ITouchMapService>("touch-map");
            routeBuilder.MapRestRoute<IDynamicService>("dynamic").WithBehaviors(new ContentTypeBehavior(homeContentTypes));
        }
    }
}
