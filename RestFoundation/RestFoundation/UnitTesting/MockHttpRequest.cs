using System;
using System.Web;

namespace RestFoundation.UnitTesting
{
    public class MockHttpRequest : Runtime.HttpRequest
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
