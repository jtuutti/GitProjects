﻿using System;
using System.Reflection;

namespace RestFoundation.Runtime
{
    internal struct ActionMethodMetadata : IEquatable<ActionMethodMetadata>
    {
        private readonly Guid m_actionMethodId;
        private readonly string m_serviceUrl;
        private readonly UrlAttribute m_urlInfo;
        private readonly MethodInfo m_methodInfo;
        private readonly OutputCacheAttribute m_outputCache;

        public ActionMethodMetadata(string serviceUrl, UrlAttribute urlInfo, MethodInfo methodInfo, OutputCacheAttribute outputCache)
        {
            if (serviceUrl == null) throw new ArgumentNullException("serviceUrl");
            if (urlInfo == null) throw new ArgumentNullException("urlInfo");
            if (methodInfo == null) throw new ArgumentNullException("methodInfo");

            m_actionMethodId = Guid.NewGuid();
            m_serviceUrl = serviceUrl.Trim();
            m_urlInfo = urlInfo;
            m_methodInfo = methodInfo;
            m_outputCache = outputCache;
        }

        public Guid ActionMethodId
        {
            get
            {
                return m_actionMethodId;
            }
        }

        public string ServiceUrl
        {
            get
            {
                return m_serviceUrl;
            }
        }

        public UrlAttribute UrlInfo
        {
            get
            {
                return m_urlInfo;
            }
        }

        public MethodInfo MethodInfo
        {
            get
            {
                return m_methodInfo;
            }
        }

        public OutputCacheAttribute OutputCache
        {
            get
            {
                return m_outputCache;
            }
        }

        public bool Equals(ActionMethodMetadata other)
        {
            return Equals(other.m_urlInfo, m_urlInfo) && Equals(other.m_methodInfo, m_methodInfo);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ActionMethodMetadata && Equals((ActionMethodMetadata) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (m_urlInfo.GetHashCode() * 397) ^ m_methodInfo.GetHashCode();
            }
        }

        public static bool operator ==(ActionMethodMetadata left, ActionMethodMetadata right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ActionMethodMetadata left, ActionMethodMetadata right)
        {
            return !left.Equals(right);
        }
    }
}
