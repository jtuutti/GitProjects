﻿// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Web;

namespace RestFoundation.UnitTesting
{
    /// <summary>
    /// Represents a mock HTTP response.
    /// </summary>
    public class MockHttpResponse : Context.HttpResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockHttpResponse"/> class.
        /// </summary>
        /// <param name="output">The HTTP response output.</param>
        public MockHttpResponse(IHttpResponseOutput output) : base(output)
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
                    throw new InvalidOperationException(Resources.Global.MissingHttpContext);
                }

                return context;
            }
        }
    }
}
