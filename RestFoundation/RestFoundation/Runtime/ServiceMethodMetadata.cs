﻿// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Reflection;
using System.Threading;

namespace RestFoundation.Runtime
{
    internal struct ServiceMethodMetadata : IEquatable<ServiceMethodMetadata>
    {
        private static int serviceMethodIdGenerator;

        private readonly int m_serviceMethodId;
        private readonly string m_serviceUrl;
        private readonly MethodInfo m_methodInfo;
        private readonly UrlAttribute m_urlInfo;

        public ServiceMethodMetadata(string serviceUrl, MethodInfo methodInfo, UrlAttribute urlInfo)
        {
            if (serviceUrl == null)
            {
                throw new ArgumentNullException("serviceUrl");
            }

            if (methodInfo == null)
            {
                throw new ArgumentNullException("methodInfo");
            }

            if (urlInfo == null)
            {
                throw new ArgumentNullException("urlInfo");
            }

            m_serviceMethodId = Interlocked.Increment(ref serviceMethodIdGenerator);
            m_serviceUrl = serviceUrl.Trim();
            m_methodInfo = methodInfo;
            m_urlInfo = urlInfo;
        }

        public int ServiceMethodId
        {
            get
            {
                return m_serviceMethodId;
            }
        }

        public string ServiceUrl
        {
            get
            {
                return m_serviceUrl;
            }
        }

        public MethodInfo MethodInfo
        {
            get
            {
                return m_methodInfo;
            }
        }

        public UrlAttribute UrlInfo
        {
            get
            {
                return m_urlInfo;
            }
        }
        
        public static bool operator ==(ServiceMethodMetadata left, ServiceMethodMetadata right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ServiceMethodMetadata left, ServiceMethodMetadata right)
        {
            return !left.Equals(right);
        }

        public bool Equals(ServiceMethodMetadata other)
        {
            return Equals(other.m_urlInfo, m_urlInfo) && Equals(other.m_methodInfo, m_methodInfo);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is ServiceMethodMetadata && Equals((ServiceMethodMetadata) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (m_urlInfo.GetHashCode() * 397) ^ m_methodInfo.GetHashCode();
            }
        }
    }
}
