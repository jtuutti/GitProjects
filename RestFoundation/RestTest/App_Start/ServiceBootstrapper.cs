using System;
using RestFoundation;
using RestFoundation.Behaviors;
using RestFoundation.Formatters;
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

            // Configuring REST foundation
            Rest.Configure.WithObjectFactory(CreateObjectFactory, CreateObjectBuilder)
                          .WithContentFormatters(RegisterFormatters)
                          .WithRoutes(RegisterRoutes)
                          .EnableJsonPSupport()
                          .WithResponseHeader("X-Service-Name", "Rest Foundation Test")
                          .WithResponseHeader("X-Service-Version", "1.0")
                          .ConfigureServiceHelpAndProxy(c => c.Enable());
        }

        private static void ConfigureIoC(ConfigurationExpression config)
        {
            config.Scan(action =>
                        {
                            action.Assembly(Rest.FoundationAssembly);
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

        public static void CreateObjectBuilder(object obj)
        {
            ObjectFactory.BuildUp(obj);
        }

        private static void RegisterFormatters(ContentFormatterBuilder builder)
        {
            builder.Set("application/bson", new BsonFormatter());
            builder.Set("application/x-www-form-urlencoded", new FormsFormatter());
            builder.Set("multipart/form-data", new MultipartFormatter());
        }

        private static void RegisterRoutes(RouteBuilder routeBuilder)
        {
            routeBuilder.MapRestRoute<IIndexService>("home")
                        .WithBehaviors(new StatisticsBehavior(), new LoggingBehavior())
                        .DoNotValidateRequests();

            routeBuilder.MapRestRouteAsync<IIndexService>("async")
                        .WithBehaviors(new BasicAuthorizationBehavior(), new StatisticsBehavior(), new LoggingBehavior());

            routeBuilder.MapRestRoute<IDynamicService>("dynamic")
                        .BlockContentType("application/x-www-form-urlencoded");

            routeBuilder.MapRestRoute<ITouchMapService>("touch-map")
                        .WithDataContractFormatters()
                        .WithBehaviors(new HttpsOnlyBehavior());

            routeBuilder.MapPageRoute("faq", "~/Views/Faq.aspx");
        }
    }
}
