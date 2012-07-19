using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using RestFoundation.Context;
using RestFoundation.Runtime;
using RestFoundation.Runtime.Handlers;
using RestFoundation.Security;
using RestFoundation.UnitTesting;

namespace RestFoundation.DependencyInjection
{
    /// <summary>
    /// Represents a dependency manager.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class DependencyManager
    {
        private readonly Dictionary<Type, Tuple<Type, DependencyLifetime>> m_dependencies;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyManager"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling",
                         Justification = "This class is response for all dependencies.")]
        public DependencyManager(bool mockContext)
        {
            m_dependencies = new Dictionary<Type, Tuple<Type, DependencyLifetime>>
            {
                { typeof(IAuthorizationManager),   Tuple.Create(typeof(AuthorizationManager),   DependencyLifetime.Singleton) },
                { typeof(IBrowserDetector),        Tuple.Create(typeof(BrowserDetector),        DependencyLifetime.Singleton) },
                { typeof(IHttpMethodResolver),     Tuple.Create(typeof(HttpMethodResolver),     DependencyLifetime.Singleton) },
                { typeof(IParameterValueProvider), Tuple.Create(typeof(ParameterValueProvider), DependencyLifetime.Singleton) },
                { typeof(IResourceValidator),      Tuple.Create(typeof(ResourceValidator),      DependencyLifetime.Singleton) },
                { typeof(IResultExecutor),         Tuple.Create(typeof(ResultExecutor),         DependencyLifetime.Singleton) },
                { typeof(IResultFactory),          Tuple.Create(typeof(ResultFactory),          DependencyLifetime.Singleton) },
                { typeof(IServiceCache),           Tuple.Create(typeof(ServiceCache),           DependencyLifetime.Singleton) },
                { typeof(IStreamCompressor),       Tuple.Create(typeof(StreamCompressor),       DependencyLifetime.Singleton) },

                { typeof(IRestAsyncHandler),       Tuple.Create(typeof(RestAsyncHandler),       DependencyLifetime.Transient) },
                { typeof(IRestHandler),            Tuple.Create(typeof(RestHandler),            DependencyLifetime.Transient) },
                { typeof(IServiceMethodInvoker),   Tuple.Create(typeof(ServiceMethodInvoker),   DependencyLifetime.Transient) },
                { typeof(IServiceMethodLocator),   Tuple.Create(typeof(ServiceMethodLocator),   DependencyLifetime.Transient) },
            };

            if (mockContext)
            {
                m_dependencies.Add(typeof(IHttpRequest),        Tuple.Create(typeof(MockHttpRequest),        DependencyLifetime.Transient));
                m_dependencies.Add(typeof(IHttpResponse),       Tuple.Create(typeof(MockHttpResponse),       DependencyLifetime.Transient));
                m_dependencies.Add(typeof(IHttpResponseOutput), Tuple.Create(typeof(MockHttpResponseOutput), DependencyLifetime.Transient));
                m_dependencies.Add(typeof(IServiceContext),     Tuple.Create(typeof(MockServiceContext),     DependencyLifetime.Transient));
            }
            else
            {
                m_dependencies.Add(typeof(IHttpRequest),        Tuple.Create(typeof(HttpRequest),        DependencyLifetime.Transient));
                m_dependencies.Add(typeof(IHttpResponse),       Tuple.Create(typeof(HttpResponse),       DependencyLifetime.Transient));
                m_dependencies.Add(typeof(IHttpResponseOutput), Tuple.Create(typeof(HttpResponseOutput), DependencyLifetime.Transient));
                m_dependencies.Add(typeof(IServiceContext),     Tuple.Create(typeof(ServiceContext),     DependencyLifetime.Transient));
            }
        }

        /// <summary>
        /// Gets a dictionary of dependencies.
        /// </summary>
        public Dictionary<Type, Tuple<Type, DependencyLifetime>> Dependencies
        {
            get
            {
                return m_dependencies;
            }
        }
    }
}
