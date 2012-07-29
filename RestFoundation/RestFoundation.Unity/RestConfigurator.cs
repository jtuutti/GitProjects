using System;
using System.Globalization;
using Microsoft.Practices.Unity;
using RestFoundation.ServiceLocation;
using RestFoundation.Unity.Properties;

namespace RestFoundation.Unity
{
    internal static class RestConfigurator
    {
        public static Rest Configure(IUnityContainer container, bool mockContext)
        {
            return Rest.Configure(CreateServiceLocator(container, mockContext));
        }

        public static IServiceLocator CreateServiceLocator(IUnityContainer container, bool mockContext)
        {
            try
            {
                var serviceBuilder = new ServiceBuilder(container);
                serviceBuilder.Build(mockContext);

                return new ServiceLocator(container);
            }
            catch (Exception ex)
            {
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, Resources.DependencyRegistrationError, ex.Message), ex);
            }
        }
    }
}
