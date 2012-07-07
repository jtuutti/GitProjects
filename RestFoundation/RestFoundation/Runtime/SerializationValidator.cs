using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace RestFoundation.Runtime
{
    internal static class SerializationValidator
    {
        public static void Validate(object value)
        {
            if (value != null && !value.GetType().IsSerializable)
            {
                throw new SerializationException(String.Format(CultureInfo.InvariantCulture,
                                                               "Object of type '{0}' is not marked as serializable. It cannot be added into cache.",
                                                               value.GetType().FullName));
            }
        }
    }
}
