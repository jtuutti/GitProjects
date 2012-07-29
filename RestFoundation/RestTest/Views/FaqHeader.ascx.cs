using System;
using System.Net;
using System.Web.UI;
using RestFoundation;

namespace RestTest.Views
{
    public partial class FaqHeader : UserControl
    {
        private readonly IServiceMethodInvoker m_invoker;
        private readonly IHttpRequest m_serviceRequest;
        private readonly IHttpResponse m_serviceResponse;

        protected FaqHeader()
        {
        }

        public FaqHeader(IServiceMethodInvoker invoker, IHttpRequest serviceRequest, IHttpResponse serviceResponse)
        {
            m_invoker = invoker;
            m_serviceRequest = serviceRequest;
            m_serviceResponse = serviceResponse;
        }

        public IServiceMethodInvoker Invoker
        {
            get
            {
                return m_invoker;
            }
        }

        public IHttpRequest ServiceRequest
        {
            get
            {
                return m_serviceRequest;
            }
        }

        public IHttpResponse ServiceResponse
        {
            get
            {
                return m_serviceResponse;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Invoker == null || ServiceRequest == null || ServiceResponse == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "Constructor dependency injection for the user control failed");
            }
        }
    }
}
