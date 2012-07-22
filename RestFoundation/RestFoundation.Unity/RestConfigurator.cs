using System;
using System.Globalization;
using Microsoft.Practices.Unity;
using RestFoundation.ServiceLocation;
using RestFoundation.Unity.Properties;

namespace RestFoundation.Unity
{
    internal static class RestConfigurator
    {
        public static Rest Configure(Action<IUnityContainer> registrationBuilder, bool mockContext)
        {
            return Rest.Configure(CreateServiceLocator(registrationBuilder, mockContext));
        }

        public static IServiceLocator CreateServiceLocator(Action<IUnityContainer> registrationBuilder, bool mockContext)
        {
            try
            {
                var container = new UnityContainer();

                var serviceBuilder = new ServiceBuilder(container);
                serviceBuilder.Build(mockContext);

                if (registrationBuilder != null)
                {
                    registrationBuilder(container);
                }

                return new ServiceLocator(container);
            }
            catch (Exception ex)
            {
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, Resources.DependencyRegistrationError, ex.Message), ex);
            }
        }
    }
}
