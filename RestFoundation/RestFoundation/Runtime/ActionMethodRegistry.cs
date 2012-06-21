using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RestFoundation.Runtime
{
    internal static class ActionMethodRegistry
    {
        private static readonly ConcurrentDictionary<ServiceMetadata, List<ActionMethodMetadata>> actionMethods = new ConcurrentDictionary<ServiceMetadata, List<ActionMethodMetadata>>();

        public static ConcurrentDictionary<ServiceMetadata, List<ActionMethodMetadata>> ActionMethods
        {
            get
            {
                return actionMethods;
            }
        }

        public static MethodInfo GetActionMethod(ServiceMetadata metadata, string urlTemplate, HttpMethod httpMethod, out OutputCacheAttribute cache)
        {
            if (metadata == null) throw new ArgumentNullException("metadata");
            if (urlTemplate == null) throw new ArgumentNullException("urlTemplate");

            List<ActionMethodMetadata> serviceActionMethods;

            if (!actionMethods.TryGetValue(metadata, out serviceActionMethods))
            {
                cache = null;
                return null;
            }

            foreach (var actionMethod in serviceActionMethods)
            {
                if (String.Equals(UrlTemplateStandardizer.Standardize(actionMethod.UrlInfo.UrlTemplate), UrlTemplateStandardizer.Standardize(urlTemplate)) &&
                    actionMethod.UrlInfo.HttpMethods.Contains(httpMethod))
                {
                    cache = actionMethod.OutputCache;
                    return actionMethod.MethodInfo;
                }
            }

            cache = null;
            return null;
        }
    }
}
