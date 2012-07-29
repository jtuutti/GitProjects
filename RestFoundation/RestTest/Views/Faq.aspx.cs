using System;
using System.Net;
using System.Web.UI;
using RestFoundation;

namespace RestTest.Views
{
    public partial class Faq : Page
    {
        private readonly IServiceContext m_serviceContext;

        // An empty protected or public constructor is required by the ASP .NET to initalize Web Forms pages
        protected Faq()
        {
        }

        public Faq(IServiceContext serviceContext)
        {
            m_serviceContext = serviceContext;
        }

        public IServiceContext ServiceContext
        {
            get
            {
                return m_serviceContext;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (ServiceContext == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "Constructor dependency injection failed");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                foreach (string queryKey in Request.QueryString.AllKeys)
                {
                    divQueryParameters.InnerHtml += String.Format("<div>{0} = {1}</div>", queryKey, Request.QueryString.Get(queryKey));
                }
            }
        }
    }
}