using System;
using System.Web;

namespace RestFoundation.Test
{
    public class MockHttpRequest : Runtime.HttpRequest
    {
        public MockHttpRequest(ICredentialResolver credentialResolver) : base(credentialResolver)
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
