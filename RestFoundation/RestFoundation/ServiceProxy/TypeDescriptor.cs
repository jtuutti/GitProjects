// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;

namespace RestFoundation.ServiceProxy
{
    internal static class TypeDescriptor
    {
        private static readonly Dictionary<Type, string> types = new Dictionary<Type, string>
        {
            { typeof(string), "string" },
            { typeof(char), "char" },
            { typeof(char?), "char" },
            { typeof(bool), "boolean" },
            { typeof(bool?), "boolean" },
            { typeof(decimal), "decimal" },
            { typeof(decimal?), "decimal" },
            { typeof(float), "float" },
            { typeof(float?), "float" },
            { typeof(double), "double" },
            { typeof(double?), "double" },
            { typeof(DateTime), "datetime" },
            { typeof(DateTime?), "datetime" },
            { typeof(long), "long" },
            { typeof(long?), "long" },
            { typeof(int), "int" },
            { typeof(int?), "int" },
            { typeof(short), "short" },
            { typeof(short?), "short" },
            { typeof(byte), "byte" },
            { typeof(byte?), "byte" },
            { typeof(ulong), "unsignedLong" },
            { typeof(ulong?), "unsignedLong" },
            { typeof(uint), "unsignedInt" },
            { typeof(uint?), "unsignedInt" },
            { typeof(ushort), "unsignedShort" },
            { typeof(ushort?), "unsignedShort" },
            { typeof(Guid), "guid" },
            { typeof(Guid?), "guid" },
            { typeof(Uri), "anyURI" }
        };

        public static string GetTypeName(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            string typeName;
            return types.TryGetValue(type, out typeName) ? typeName : "object";
        }
    }
}
