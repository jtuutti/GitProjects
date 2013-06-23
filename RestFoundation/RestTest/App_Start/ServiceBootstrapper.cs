using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using RestFoundation;
using RestFoundation.Behaviors;
using RestFoundation.Configuration;
using RestFoundation.Formatters;
using RestFoundation.Security;
using RestTest.Handlers;
using RestTest.Security;
using RestTest.SimpleServices;
using RestTest.StreamCompressors;
using RestTestContracts;
using RestTestContracts.Behaviors;
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
                .WithXmlFormatterSettings(new XmlFormatterSettings { Namespace = "urn:com.rest-test.resources" })
                .WithResponseHeader("X-Service-Name", "Rest Foundation Test")
                .WithResponseHeader("X-Service-Version", "1.0")
                .EnableJsonPSupport()               
                .ConfigureServiceHelpAndProxy(c => c.Enable()
                                                    .WithServiceDescription("RESTFul test service")
                                                    .RequireAuthorization(new ProxyAuthorizationManager()));
        }

        private static IUnityContainer CreateUnityContainer()
        {
            var container = new UnityContainer();
            container.RegisterType<IAuthorizationManager, ServiceAuthorizationManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<IStreamCompressor, RestStreamCompressor>(new ContainerControlledLifetimeManager());
            container.RegisterType<IIndexService, IndexService>(new InjectionProperty("Context"));
            container.RegisterType<IDynamicService, DynamicService>(new InjectionProperty("Request"));
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

                registry.SetAllProperties(convention => convention.TypeMatches(type => Rest.ServiceContextTypes.Contains(type)));
            });
        }

        private static void RegisterServiceDependencies(DependencyBuilder builder)
        {
            builder.Register<IAuthorizationManager, ServiceAuthorizationManager>(InstanceLifetime.Singleton)
                   .Register<IStreamCompressor, RestStreamCompressor>(InstanceLifetime.Singleton)
                   .ScanAssemblies(new[] { "RestTestContracts", "RestTestServices" }, t => t.Name.EndsWith("Service"))
                   .AllowPropertyInjection(t => t.GetProperty("Context") != null || t.GetProperty("Request") != null);
        }

        private static void RegisterFormatters(MediaTypeFormatterBuilder builder)
        {
            builder.Set(new FormsFormatter());
        }

        private static void RegisterUrls(UrlBuilder urlBuilder)
        {
            urlBuilder.MapUrl("home")
                      .ToServiceContract<IIndexService>()
                      .WithBehaviors(new StatisticsBehavior(), new LoggingBehavior())
                      .DoNotValidateRequests();

            urlBuilder.MapUrl("secure")
                      .ToServiceContract<IIndexService>()
                      .WithBehaviors(new DigestAuthenticationBehavior(), new AuthorizationBehavior("administrators"), new StatisticsBehavior(), new LoggingBehavior());

            urlBuilder.MapUrl("dynamic")
                      .ToServiceContract<IDynamicService>()
                      .BlockMediaTypesForFormatter<FormsFormatter>();

            urlBuilder.MapUrl("touch-map")
                      .ToServiceContract<ITouchMapService>()
                      .WithBehaviors(new HttpsOnlyBehavior());

            urlBuilder.MapUrl("hello")
                      .ToServiceContract<HelloService>();

            urlBuilder.MapUrl("secure-hello")
                      .ToServiceContract<HelloService>().WithBehaviors(new CustomHmacAuthenticationBehavior());

            urlBuilder.MapUrl("feed.{format}")
                      .ToHttpHandler<FeedHandler>(null, new RouteHash(format => "^atom|rss$"));

            urlBuilder.MapUrl("download")
                      .ToHttpHandler<DownloadHandler>();

            urlBuilder.MapUrl("upload")
                      .ToHttpHandler<UploadHandler>();

            urlBuilder.MapUrl("faq")
                      .ToWebFormsPage("~/Views/Faq.aspx");
        }
    }
}
