using System;
using System.Globalization;
using RestFoundation.ServiceLocation;
using RestFoundation.StructureMap.Properties;
using StructureMap;

namespace RestFoundation.StructureMap
{
    internal static class RestConfigurator
    {
        public static Rest Configure(IContainer container, bool mockContext)
        {
            return Rest.Configure(CreateServiceLocator(container, mockContext));
        }

        public static IServiceLocator CreateServiceLocator(IContainer container, bool mockContext)
        {
            try
            {
                var serviceBuilder = new ServiceBuilder(container);
                serviceBuilder.Build(mockContext);

#if DEBUG
                container.AssertConfigurationIsValid();
#endif

                return new ServiceLocator(container);
            }
            catch (Exception ex)
            {
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, Resources.DependencyRegistrationError, ex.Message), ex);
            }
        }
    }
}
