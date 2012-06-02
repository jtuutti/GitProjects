using System;

namespace MvcAlt.Infrastructure
{
    public static class TypeHelper
    {
        public static bool IsSimpleType(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return type.IsPrimitive || type.IsEnum || type == typeof(Guid) || type == typeof(decimal) ||
                   (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}
