using System;
using System.Web;
using RestFoundation.Context;

namespace RestFoundation.UnitTesting
{
    public class MockServiceContext : ServiceContext
    {
        public MockServiceContext(IHttpRequest request, IHttpResponse response) : base(request, response)
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
