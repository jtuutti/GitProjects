using System;
using Microsoft.Practices.ServiceLocation;

namespace RestFoundation
{
    internal static class ObjectActivator
    {
        public static T Create<T>()
        {
            ValidateActiveServiceLocator();

            try
            {
                return ServiceLocator.Current.GetInstance<T>();
            }
            catch (Exception ex)
            {
                throw new ActivationException(String.Format("Object of type '{0}' could not be initialized", typeof(T).FullName), ex);
            }
        }

        public static object Create(Type objectType)
        {
            ValidateActiveServiceLocator();

            try
            {
                return ServiceLocator.Current.GetInstance(objectType);
            }
            catch (Exception ex)
            {
                throw new ActivationException(String.Format("Object of type '{0}' could not be initialized", objectType.FullName), ex);
            }
        }

        private static void ValidateActiveServiceLocator()
        {
            try
            {
                if (ServiceLocator.Current == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                throw new ActivationException("No service locator is available. You can set it by calling the " +
                                              "'Microsoft.Practices.ServiceLocation.ServiceLocator.SetLocatorProvider(newProvider)' function " +
                                              "from the 'Application_Start' method in the 'Global.asax' file.");
            }
        }
    }
}
