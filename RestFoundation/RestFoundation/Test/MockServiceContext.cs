using System;
using System.Web;
using RestFoundation.Runtime;

namespace RestFoundation.Test
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
                HttpContextBase context = MockContextFactory.Context;

                if (context == null)
                {
                    throw new InvalidOperationException("No HTTP context was found");
                }

                return context;
            }
        }
    }
}
