using System;

namespace RestFoundation.UnitTesting
{
    /// <summary>
    /// Contains REST configuration extensions.
    /// </summary>
    public static class RestExtensions
    {
        /// <summary>
        /// Returns a concrete implementation associated with the provided interface.
        /// </summary>
        /// <param name="restConfiguration">The REST configuration object.</param>
        /// <param name="objectType">The object type.</param>
        /// <returns>The implementation instance.</returns>
        public static object GetImplementation(this Rest restConfiguration, Type objectType)
        {
            if (restConfiguration == null) throw new ArgumentNullException("restConfiguration");

            if (objectType == null || !objectType.IsInterface)
            {
                throw new ObjectActivationException("The object type provided is not an interface");
            }

            return Rest.Active.CreateObject(objectType);
        }

        /// <summary>
        /// Returns a concrete implementation associated with the provided interface.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="restConfiguration">The REST configuration object.</param>
        /// <returns>The implementation instance.</returns>
        public static T GetImplementation<T>(this Rest restConfiguration)
        {
            if (restConfiguration == null) throw new ArgumentNullException("restConfiguration");

            return Rest.Active.CreateObject<T>();
        }
    }
}
