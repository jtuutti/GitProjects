using System;
using System.Linq;
using StructureMap;

namespace RestFoundation.StructureMap
{
    internal static class StructureMapExtensions
    {
        public static bool IsRegistered(this IContainer container, Type serviceType)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }

            return container.Model.PluginTypes.Any(p => p.PluginType == serviceType);
        }
    }
}
