// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Threading.Tasks;

namespace RestFoundation.Runtime
{
    internal static class TypeExtensions
    {
        private static readonly Type genericTaskType = typeof(Task<>);
        private static readonly Type voidTaskType = typeof(Task);

        public static bool IsGenericTask(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == genericTaskType)
            {
                return true;
            }

            return type.BaseType != null && IsGenericTask(type.BaseType);
        }

        public static bool IsVoidTask(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (type.GetGenericTypeDefinition() == voidTaskType)
            {
                return true;
            }

            return type.BaseType != null && IsGenericTask(type.BaseType);
        }
    }
}
