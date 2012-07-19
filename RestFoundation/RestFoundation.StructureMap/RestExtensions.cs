using System;
using System.Globalization;
using RestFoundation.DependencyInjection;
using RestFoundation.StructureMap;
using RestFoundation.StructureMap.Properties;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace RestFoundation
{
    public static class RestExtensions
    {
        public static Rest ConfigureWithStructureMap(this Rest restConfiguration)
        {
            return Configure(null, false);
        }

        public static Rest ConfigureWithStructureMap(this Rest restConfiguration, Action<Registry> registrationBuilder)
        {
            return Configure(registrationBuilder, false);
        }

        public static Rest ConfigureMocksWithStructureMap(this Rest restConfiguration)
        {
            return Configure(null, true);
        }

        public static Rest ConfigureMocksWithStructureMap(this Rest restConfiguration, Action<Registry> registrationBuilder)
        {
            return Configure(registrationBuilder, true);
        }

        private static Rest Configure(Action<Registry> registrationBuilder, bool mockContext)
        {
            try
            {
                var registry = new Registry();

                RegisterDependencies(registry, mockContext);
                registry.SetAllProperties(convention => convention.TypeMatches(type => type.IsRestDependency()));

                if (registrationBuilder != null)
                {
                    registrationBuilder(registry);
                }

                var container = new Container(registry);

#if DEBUG
                container.AssertConfigurationIsValid();
#endif

                return Rest.Configure(new DependencyResolver(container));
            }
            catch (Exception ex)
            {
                throw new DependencyInjectionException(String.Format(CultureInfo.InvariantCulture, Resources.DependencyRegistrationError, ex.Message), ex);
            }
        }

        private static void RegisterDependencies(Registry registry, bool mockContext)
        {
            var dependencyManager = new DependencyManager(mockContext);
            var dependencyRegistry = new DependencyRegistry(registry);

            foreach (var dependency in dependencyManager.Dependencies)
            {
                dependencyRegistry.Register(dependency.Key, dependency.Value.Item1, dependency.Value.Item2, Rest.RestKey);
            }
        }
    }
}
