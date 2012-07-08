using System;
using System.Web;
using RestFoundation.Context;

namespace RestFoundation.UnitTesting
{
    public class MockHttpResponseOutput : HttpResponseOutput
    {
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
