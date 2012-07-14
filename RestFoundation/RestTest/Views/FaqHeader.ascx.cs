using System;
using System.Net;
using System.Web.UI;
using RestFoundation;

namespace RestTest.Views
{
    public partial class FaqHeader : UserControl
    {
        private readonly IServiceMethodInvoker m_invoker;

        protected FaqHeader()
        {
        }

        public FaqHeader(IServiceMethodInvoker invoker)
        {
            m_invoker = invoker;
        }

        public IServiceMethodInvoker Invoker
        {
            get
            {
                return m_invoker;
            }
        }

        public IHttpRequest ServiceRequest { get; set; }
        public IHttpResponse ServiceResponse { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Invoker == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "Constructor dependency injection for the user control failed");
            }

            if (ServiceRequest == null || ServiceResponse == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "Property dependency injection for the user control failed");
            }
        }
    }
}
