// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using RestFoundation.Configuration;
using RestFoundation.ServiceLocation;
using StructureMap;

namespace RestFoundation.StructureMap
{
    internal static class RestConfigurator
    {
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
                         Justification = "The service locator will be disposed by the application.")]
        public static RestOptions Configure(IContainer container, bool mockContext)
        {
            return Rest.Configuration.Initialize(CreateServiceLocator(container, mockContext));
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
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, Properties.Resources.DependencyRegistrationError, ex.Message), ex);
            }
        }
    }
}
