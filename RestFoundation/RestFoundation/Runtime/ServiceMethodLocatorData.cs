using System;
using System.Reflection;

namespace RestFoundation.Runtime
{
    public sealed class ServiceMethodLocatorData
    {
        public static readonly ServiceMethodLocatorData Options = new ServiceMethodLocatorData();

        private ServiceMethodLocatorData()
        {
        }

        public ServiceMethodLocatorData(object service, MethodInfo method)
        {
            if (service == null) throw new ArgumentNullException("service");
            if (method == null) throw new ArgumentNullException("method");

            Service = service;
            Method = method;
        }

        public object Service { get; private set; }
        public MethodInfo Method { get; private set; }
    }
}
