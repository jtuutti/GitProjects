using System;
using System.Net;
using System.Web.UI;
using RestFoundation;

namespace RestTest.Views
{
    public partial class Faq : Page
    {
        public IServiceContext ServiceContext { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (ServiceContext == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "Dependency injection failed");
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