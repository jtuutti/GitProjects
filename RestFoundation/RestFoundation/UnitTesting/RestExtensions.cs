using System;

namespace RestFoundation.UnitTesting
{
    public static class RestExtensions
    {       
        public static object GetImplementation(this Rest restConfiguration, Type objectType)
        {
            if (restConfiguration == null) throw new ArgumentNullException("restConfiguration");

            return Rest.Active.CreateObject(objectType);
        }

        public static T GetImplementation<T>(this Rest restConfiguration)
        {
            if (restConfiguration == null) throw new ArgumentNullException("restConfiguration");

            return Rest.Active.CreateObject<T>();
        }
    }
}
