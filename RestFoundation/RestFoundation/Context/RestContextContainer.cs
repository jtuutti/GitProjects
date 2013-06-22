// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections;
using RestFoundation.Collections;

namespace RestFoundation.Context
{
    internal sealed class RestContextContainer
    {
        private const string ContextContainerKey = "REST_Context";

        private readonly object m_syncRoot = new Object();
        private readonly Func<IDictionary> m_contextItemBuilder;

        public RestContextContainer(Func<IDictionary> contextItemBuilder)
        {
            if (contextItemBuilder == null)
            {
                throw new ArgumentNullException("contextItemBuilder");
            }

            m_contextItemBuilder = contextItemBuilder;
        }

        public ICookieValueCollection Cookies
        {
            get
            {
                return ContextValues["Cookies"] as ICookieValueCollection;
            }
            set
            {
                ContextValues["Cookies"] = value;
            }
        }

        public IStringValueCollection Form
        {
            get
            {
                return ContextValues["Form"] as IStringValueCollection;
            }
            set
            {
                ContextValues["Form"] = value;
            }
        }

        public IHeaderCollection Headers
        {
            get
            {
                return ContextValues["Headers"] as IHeaderCollection;
            }
            set
            {
                ContextValues["Headers"] = value;
            }
        }

        public HttpMethod? Method
        {
            get
            {
                return ContextValues["Method"] as HttpMethod?;
            }
            set
            {
                ContextValues["Method"] = value;
            }
        }

        public IStringValueCollection QueryString
        {
            get
            {
                return ContextValues["QueryString"] as IStringValueCollection;
            }
            set
            {
                ContextValues["QueryString"] = value;
            }
        }

        public IRouteValueCollection RouteValues
        {
            get
            {
                return ContextValues["RouteValues"] as IRouteValueCollection;
            }
            set
            {
                ContextValues["RouteValues"] = value;
            }
        }

        public IServerVariableCollection ServerVariables
        {
            get
            {
                return ContextValues["ServerVariables"] as IServerVariableCollection;
            }
            set
            {
                ContextValues["ServerVariables"] = value;
            }
        }

        public ServiceOperationUri Uri
        {
            get
            {
                return ContextValues["Uri"] as ServiceOperationUri;
            }
            set
            {
                ContextValues["Uri"] = value;
            }
        }

        private Hashtable ContextValues
        {
            get
            {
                IDictionary contextItems = m_contextItemBuilder();

                var contextValues = contextItems[ContextContainerKey] as Hashtable;

                if (contextValues != null)
                {
                    return contextValues;
                }

                lock (m_syncRoot)
                {
                    contextItems[ContextContainerKey] = new Hashtable();
                    return (Hashtable) contextItems[ContextContainerKey];
                }
            }
        }
    }
}
