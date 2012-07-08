using System;
using System.Web;

namespace RestFoundation.UnitTesting
{
    public class MockHttpResponse : Context.HttpResponse
    {
        public MockHttpResponse(IHttpResponseOutput output) : base(output)
        {
        }

        protected override HttpContextBase Context
        {
            get
            {
                HttpContextBase context = MockHandlerFactory.Context;

                if (context == null)
                {
                    throw new InvalidOperationException("No HTTP context was found");
                }

                return context;
            }
        }
    }
}
