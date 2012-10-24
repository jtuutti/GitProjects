// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Practices.Unity;
using RestFoundation.ServiceLocation;
using RestFoundation.Unity.Properties;

namespace RestFoundation.Unity
{
    internal static class RestConfigurator
    {
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
                         Justification = "The service locator will be disposed by the application.")]
        public static RestOptions Configure(IUnityContainer container, bool mockContext)
        {
            return Rest.Configuration.Initialize(CreateServiceLocator(container, mockContext));
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
