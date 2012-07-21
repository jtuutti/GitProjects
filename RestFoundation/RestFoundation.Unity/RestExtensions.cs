using System;
using System.Globalization;
using Microsoft.Practices.Unity;
using RestFoundation.DependencyInjection;
using RestFoundation.Unity.Properties;

namespace RestFoundation.Unity
{
    public static class RestExtensions
    {
        public static Rest ConfigureWithUnity(this Rest restConfiguration)
        {
            return Configure(null, false);
        }

        public static Rest ConfigureWithUnity(this Rest restConfiguration, Action<IUnityContainer> registrationBuilder)
        {
            return Configure(registrationBuilder, false);
        }

        public static Rest ConfigureMocksWithUnity(this Rest restConfiguration)
        {
            return Configure(null, true);
        }

        public static Rest ConfigureMocksWithUnity(this Rest restConfiguration, Action<IUnityContainer> registrationBuilder)
        {
            return Configure(registrationBuilder, true);
        }

        private static Rest Configure(Action<IUnityContainer> registrationBuilder, bool mockContext)
        {
            try
            {
                var container = new UnityContainer();

                RegisterDependencies(container, mockContext);

                if (registrationBuilder != null)
                {
                    registrationBuilder(container);
                }

                return Rest.Configure(new DependencyResolver(container));
            }
            catch (Exception ex)
            {
                throw new DependencyInjectionException(String.Format(CultureInfo.InvariantCulture, Resources.DependencyRegistrationError, ex.Message), ex);
            }
        }

        private static void RegisterDependencies(IUnityContainer container, bool mockContext)
        {
            var dependencyManager = new DependencyManager(mockContext);
            var dependencyRegistry = new DependencyRegistry(container);

            foreach (var dependency in dependencyManager.Dependencies)
            {
                dependencyRegistry.Register(dependency.Key, dependency.Value.Item1, dependency.Value.Item2, null);
            }
        }
    }
}
