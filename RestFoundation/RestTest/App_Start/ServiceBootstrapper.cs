using Microsoft.Practices.Unity;
using RestFoundation;
using RestFoundation.Behaviors;
using RestFoundation.Formatters;
using RestFoundation.Unity;
using RestTest.Security;
using RestTest.Behaviors;
using RestTest.StreamCompressors;
using RestTestContracts;
using RestTestServices;
using StructureMap.Configuration.DSL;

namespace RestTest.App_Start
{
    public static class ServiceBootstrapper
    {
        public static void RegisterDependencies()
        {
            Rest.Active.ConfigureWithStructureMap(RegisterDependencies)
                       .WithContentFormatters(RegisterFormatters)
                       .WithRoutes(RegisterRoutes)
                       .EnableJsonPSupport()
                       .WithResponseHeader("X-Service-Name", "Rest Foundation Test")
                       .WithResponseHeader("X-Service-Version", "1.0")
                       .ConfigureServiceHelpAndProxy(c => c.Enable());
        }

        private static void RegisterDependencies(IUnityContainer container)
        {
            container.RegisterType<IAuthorizationManager, ServiceAuthorizationManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<IStreamCompressor, RestStreamCompressor>(new ContainerControlledLifetimeManager());

            container.RegisterType<IIndexService, IndexService>(new InjectionProperty("Context"));
            container.RegisterType<IDynamicService, DynamicService>();
            container.RegisterType<ITouchMapService, TouchMapService>(new InjectionProperty("Context"));
        }

        private static void RegisterDependencies(Registry registry)
        {
            registry.ForSingletonOf<IAuthorizationManager>().Use<ServiceAuthorizationManager>();
            registry.ForSingletonOf<IStreamCompressor>().Use<RestStreamCompressor>();

            registry.Scan(action =>
            {
                action.Assembly("RestTestContracts");
                action.Assembly("RestTestServices");
                action.WithDefaultConventions();
            });
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
                        .WithBehaviors(new DigestAuthorizationBehavior(), new StatisticsBehavior(), new LoggingBehavior());

            routeBuilder.MapRestRoute<IDynamicService>("dynamic")
                        .BlockContentType("application/x-www-form-urlencoded");

            routeBuilder.MapRestRoute<ITouchMapService>("touch-map")
                        .WithDataContractFormatters()
                        .WithBehaviors(new HttpsOnlyBehavior());

            routeBuilder.MapPageRoute("faq", "~/Views/Faq.aspx");
        }
    }
}
