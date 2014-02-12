// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Practices.Unity;
using RestFoundation.Configuration;
using RestFoundation.ServiceLocation;

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
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, Properties.Resources.DependencyRegistrationError, ex.Message), ex);
            }
        }
    }
}
