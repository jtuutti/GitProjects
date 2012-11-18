using Microsoft.Practices.Unity;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using RestFoundation;
using RestFoundation.Behaviors;
using RestFoundation.Formatters;
using RestFoundation.Security;
using RestTest.Behaviors;
using RestTest.Security;
using RestTest.SimpleServices;
using RestTest.StreamCompressors;
using RestTestContracts;
using RestTestServices;
using StructureMap;

namespace RestTest.App_Start
{
    public static class ServiceBootstrapper
    {
        public static void RegisterDependencies()
        {
            DynamicModuleUtility.RegisterModule(typeof(RestHttpModule));

            Rest.Configuration
                .Initialize(RegisterServiceDependencies)
                // .InitializeWithStructureMap(CreateStructureMapContainer())
                // .InitializeWithUnity(CreateUnityContainer())
                .WithUrls(RegisterUrls)
                .WithMediaTypeFormatters(RegisterFormatters)
                .EnableJsonPSupport()
                .WithResponseHeader("X-Service-Name", "Rest Foundation Test")
                .WithResponseHeader("X-Service-Version", "1.0")
                .ConfigureServiceHelpAndProxy(c => c.Enable().RequireAuthorization(new ProxyAuthorizationManager()));
        }

        private static IUnityContainer CreateUnityContainer()
        {
            var container = new UnityContainer();
            container.RegisterType<IAuthorizationManager, ServiceAuthorizationManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<IStreamCompressor, RestStreamCompressor>(new ContainerControlledLifetimeManager());
            container.RegisterType<IIndexService, IndexService>(new InjectionProperty("Context"));
            container.RegisterType<IDynamicService, DynamicService>();
            container.RegisterType<ITouchMapService, TouchMapService>(new InjectionProperty("Context"));

            return container;
        }

        private static IContainer CreateStructureMapContainer()
        {
            return new Container(registry =>
            {
                registry.ForSingletonOf<IAuthorizationManager>().Use<ServiceAuthorizationManager>();
                registry.ForSingletonOf<IStreamCompressor>().Use<RestStreamCompressor>();

                registry.Scan(scanner =>
                {
                    scanner.Assembly("RestTestContracts");
                    scanner.Assembly("RestTestServices");
                    scanner.WithDefaultConventions();
                });

                registry.SetAllProperties(convention => convention.TypeMatches(Rest.ServiceContextTypes.Contains));
            });
        }

        private static void RegisterServiceDependencies(DependencyBuilder builder)
        {
            builder.Register<IAuthorizationManager, ServiceAuthorizationManager>(InstanceLifetime.Singleton)
                   .Register<IStreamCompressor, RestStreamCompressor>(InstanceLifetime.Singleton)
                   .ScanAssemblies(new[] { "RestTestContracts", "RestTestServices" }, t => t.Name.EndsWith("Service"))
                   .AllowPropertyInjection(t =>
                   {
                       var contextProperty = t.GetProperty("Context");
                       return contextProperty != null && contextProperty.PropertyType == typeof(IServiceContext);
                   });
        }

        private static void RegisterFormatters(MediaTypeFormatterBuilder builder)
        {
            builder.Set("application/bson", new BsonFormatter());
            builder.Set("application/x-www-form-urlencoded", new FormsFormatter());
            builder.Set("multipart/form-data", new MultipartFormatter());
        }

        private static void RegisterUrls(UrlBuilder urlBuilder)
        {
            urlBuilder.MapUrl("home")
                      .ToServiceContract<IIndexService>()
                      .WithBehaviors(new StatisticsBehavior(), new LoggingBehavior())
                      .DoNotValidateRequests();

            urlBuilder.MapUrl("async")
                      .WithAsyncHandler()
                      .ToServiceContract<IIndexService>()
                      .WithBehaviors(new DigestAuthenticationBehavior(), new AuthorizationBehavior("administrators"), new StatisticsBehavior(), new LoggingBehavior());

            urlBuilder.MapUrl("dynamic")
                      .ToServiceContract<IDynamicService>()
                      .BlockMediaType("application/x-www-form-urlencoded");

            urlBuilder.MapUrl("touch-map")
                      .ToServiceContract<ITouchMapService>()
                      .WithDataContractFormatters()
                      .WithBehaviors(new HttpsOnlyBehavior());

            urlBuilder.MapUrl("hello")
                      .ToServiceContract<HelloService>();

            urlBuilder.MapUrl("faq")
                      .ToWebFormsPage("~/Views/Faq.aspx");
        }
    }
}
