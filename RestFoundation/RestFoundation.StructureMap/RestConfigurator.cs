using System;
using System.Globalization;
using RestFoundation.ServiceLocation;
using RestFoundation.StructureMap.Properties;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace RestFoundation.StructureMap
{
    internal static class RestConfigurator
    {
        public static Rest Configure(Action<Registry> registrationBuilder, bool mockContext)
        {
            try
            {
                var registry = new Registry();

                var serviceBuilder = new ServiceBuilder(registry);
                serviceBuilder.Build(mockContext);

                registry.SetAllProperties(convention => convention.TypeMatches(type => type.IsRestDependency()));

                if (registrationBuilder != null)
                {
                    registrationBuilder(registry);
                }

                var container = new Container(registry);

#if DEBUG
                container.AssertConfigurationIsValid();
#endif

                return Rest.Configure(new ServiceLocator(container));
            }
            catch (Exception ex)
            {
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, Resources.DependencyRegistrationError, ex.Message), ex);
            }
        }
    }
}
