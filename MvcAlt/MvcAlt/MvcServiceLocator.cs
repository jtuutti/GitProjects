using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using StructureMap;

namespace MvcAlt
{
    public class MvcServiceLocator : IServiceLocator
    {
        private const string ActivationExceptionMessage = "There was an exception locating service of type '{0}'";

        private readonly IContainer container;

        public MvcServiceLocator(IContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");

            this.container = container;
        }

        public object GetInstance(Type serviceType)
        {
            return GetInstance(serviceType, null);
        }

        public object GetInstance(Type serviceType, string key)
        {
            if (serviceType == null) throw new ArgumentNullException("serviceType");

            try
            {
                if (serviceType.IsAbstract || serviceType.IsInterface)
                {
                    return key != null ? container.TryGetInstance(serviceType, key) : container.TryGetInstance(serviceType);
                }

                return key != null ? container.GetInstance(serviceType, key) : container.GetInstance(serviceType);
            }
            catch (Exception ex)
            {
                throw new ActivationException(String.Format(ActivationExceptionMessage, serviceType.FullName), ex);
            }
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException("serviceType");

            try
            {
                return container.GetAllInstances(serviceType).Cast<object>();
            }
            catch (Exception ex)
            {
                throw new ActivationException(String.Format(ActivationExceptionMessage, serviceType.FullName), ex);
            }
        }

        public TService GetInstance<TService>()
        {
            return GetInstance<TService>(null);
        }

        public TService GetInstance<TService>(string key)
        {
            Type serviceType = typeof(TService);

            try
            {
                if (serviceType.IsAbstract || serviceType.IsInterface)
                {
                    return key != null ? container.TryGetInstance<TService>(key) : container.TryGetInstance<TService>();
                }

                return key != null ? container.GetInstance<TService>(key) : container.GetInstance<TService>();
            }
            catch (Exception ex)
            {
                throw new ActivationException(String.Format(ActivationExceptionMessage, serviceType.FullName), ex);
            }
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            try
            {
                return container.GetAllInstances<TService>();
            }
            catch (Exception ex)
            {
                throw new ActivationException(String.Format(ActivationExceptionMessage, typeof(TService).FullName), ex);
            }
        }
 
        object IServiceProvider.GetService(Type serviceType)
        {
            return GetInstance(serviceType, null);
        }
   }
}