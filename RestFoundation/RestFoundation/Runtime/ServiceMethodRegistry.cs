﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RestFoundation.Runtime
{
    internal static class ServiceMethodRegistry
    {
        private static readonly ConcurrentDictionary<ServiceMetadata, List<ServiceMethodMetadata>> serviceMethods = new ConcurrentDictionary<ServiceMetadata, List<ServiceMethodMetadata>>();

        public static ConcurrentDictionary<ServiceMetadata, List<ServiceMethodMetadata>> ServiceMethods
        {
            get
            {
                return serviceMethods;
            }
        }

        public static MethodInfo GetMethod(ServiceMetadata metadata, string urlTemplate, HttpMethod httpMethod, out OutputCacheAttribute cache)
        {
            if (metadata == null) throw new ArgumentNullException("metadata");
            if (urlTemplate == null) throw new ArgumentNullException("urlTemplate");

            List<ServiceMethodMetadata> methods;

            if (!serviceMethods.TryGetValue(metadata, out methods))
            {
                cache = null;
                return null;
            }

            foreach (var method in methods)
            {
                if (String.Equals(UrlTemplateStandardizer.Standardize(method.UrlInfo.UrlTemplate), UrlTemplateStandardizer.Standardize(urlTemplate)) &&
                    method.UrlInfo.HttpMethods.Contains(httpMethod))
                {
                    cache = method.OutputCache;
                    return method.MethodInfo;
                }
            }

            cache = null;
            return null;
        }
    }
}
