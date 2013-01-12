// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Web;
using RestFoundation.Context;

namespace RestFoundation.UnitTesting
{
    /// <summary>
    /// Represents a mock HTTP response output.
    /// </summary>
    public class MockHttpResponseOutput : HttpResponseOutput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockHttpResponseOutput"/> class.
        /// </summary>
        /// <param name="logWriter">The log writer.</param>
        public MockHttpResponseOutput(ILogWriter logWriter) : base(logWriter)
        {
        }

        /// <summary>
        /// Gets the underlying <see cref="HttpContextBase"/> instance.
        /// </summary>
        protected override HttpContextBase Context
        {
            get
            {
                HttpContextBase context = TestHttpContext.Context;

                if (context == null)
                {
                    throw new InvalidOperationException(RestResources.MissingHttpContext);
                }

                return context;
            }
        }
    }
}
