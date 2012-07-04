using System;
using System.Web;
using RestFoundation.Runtime;

namespace RestFoundation.UnitTesting
{
    public class MockHttpResponseOutput : HttpResponseOutput
    {
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
