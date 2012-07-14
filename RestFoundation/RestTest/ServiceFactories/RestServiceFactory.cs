using System;
using RestFoundation;
using StructureMap;

namespace RestTest.ServiceFactories
{
    public class RestServiceFactory : IServiceFactory
    {
        private static readonly IContainer serviceContainer;

        static RestServiceFactory()
        {
            IContainer container = ObjectFactory.Container.GetNestedContainer();

            container.Configure(config => config.Scan(action =>
                                                      {
                                                          action.Assembly("RestTestContracts");
                                                          action.Assembly("RestTestServices");
                                                          action.WithDefaultConventions();
                                                      }));

            serviceContainer = container;
        }

        public static IContainer ServiceContainer
        {
            get
            {
                return serviceContainer;
            }
        }

        public object Create(Type serviceContractType, IServiceContext context)
        {
            return serviceContainer.TryGetInstance(serviceContractType);
        }
    }
}
