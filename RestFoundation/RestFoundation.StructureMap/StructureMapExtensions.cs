using System;
using System.Linq;
using StructureMap;

namespace RestFoundation.StructureMap
{
    internal static class StructureMapExtensions
    {
        public static bool IsRegistered<T>(this IContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            Type abstractionType = typeof(T);

            return container.Model.PluginTypes.Any(p => p.PluginType == abstractionType);
        }
    }
}
