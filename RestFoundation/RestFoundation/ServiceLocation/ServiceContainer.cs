// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using RestFoundation.Client;
using RestFoundation.Client.Serializers;
using RestFoundation.Context;
using RestFoundation.Results;
using RestFoundation.Runtime;
using RestFoundation.Runtime.Handlers;
using RestFoundation.Security;
using RestFoundation.UnitTesting;
using RestFoundation.Validation;

namespace RestFoundation.ServiceLocation
{
    /// <summary>
    /// Represents an internal service container.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class ServiceContainer
    {
        private readonly Dictionary<Type, Type> m_singletonServices;
        private readonly Dictionary<Type, Type> m_transientServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceContainer"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling",
                         Justification = "This class is a service container, therefore it is coupled to numerous implementations.")]
        public ServiceContainer(bool mockContext)
        {
            m_singletonServices = new Dictionary<Type, Type>
            {
                { typeof(IAuthorizationManager), typeof(AuthorizationManager) },
                { typeof(IContentNegotiator), typeof(ContentNegotiator) },
                { typeof(IHttpMethodResolver), typeof(HttpMethodResolver) },
                { typeof(IParameterValueProvider), typeof(ParameterValueProvider) },
                { typeof(IResourceValidator), typeof(ResourceValidator) },
                { typeof(IRestSerializerFactory), typeof(RestSerializerFactory) },
                { typeof(IResultExecutor), typeof(ResultExecutor) },
                { typeof(IResultFactory), typeof(ResultFactory) },
                { typeof(IServiceCache), typeof(ServiceCache) },
                { typeof(IStreamCompressor), typeof(StreamCompressor) }
            };

            m_transientServices = new Dictionary<Type, Type>
            {
                { typeof(IRestAsyncHandler), typeof(RestAsyncHandler) },
                { typeof(IRestHandler), typeof(RestHandler) },
                { typeof(IServiceBehaviorInvoker), typeof(ServiceBehaviorInvoker) },
                { typeof(IServiceMethodInvoker), typeof(ServiceMethodInvoker) },
                { typeof(IServiceMethodLocator), typeof(ServiceMethodLocator) }
            };

            if (mockContext)
            {
                m_transientServices.Add(typeof(IHttpRequest), typeof(MockHttpRequest));
                m_transientServices.Add(typeof(IHttpResponse), typeof(MockHttpResponse));
                m_transientServices.Add(typeof(IHttpResponseOutput), typeof(MockHttpResponseOutput));
                m_transientServices.Add(typeof(IServiceContext), typeof(MockServiceContext));
            }
            else
            {
                m_transientServices.Add(typeof(IHttpRequest), typeof(HttpRequest));
                m_transientServices.Add(typeof(IHttpResponse), typeof(HttpResponse));
                m_transientServices.Add(typeof(IHttpResponseOutput), typeof(HttpResponseOutput));
                m_transientServices.Add(typeof(IServiceContext), typeof(ServiceContext));
            }
        }

        /// <summary>
        /// Gets a dictionary of singleton service types.
        /// </summary>
        public Dictionary<Type, Type> SingletonServices
        {
            get
            {
                return m_singletonServices;
            }
        }

        /// <summary>
        /// Gets a dictionary of transient service types.
        /// </summary>
        public Dictionary<Type, Type> TransientServices
        {
            get
            {
                return m_transientServices;
            }
        }
    }
}
