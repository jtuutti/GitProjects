using System;
using System.Reflection;
using System.Threading.Tasks;

namespace RestFoundation.Runtime
{
    internal static class MethodReturnTypeUnwrapper
    {
        public static Type Unwrap(Type methodReturnType)
        {
            if (methodReturnType == null || methodReturnType == typeof(Task))
            {
                return typeof(void);
            }

            if (methodReturnType.IsGenericType() && methodReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                return methodReturnType.GetGenericArguments()[0];
            }

            return methodReturnType;
        }
    }
}
