﻿// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Net;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents a web client for the service proxy.
    /// </summary>
    public sealed class ProxyWebClient : WebClient
    {
        private const string IfModifiedSinceHeader = "If-Modified-Since";

        private bool m_isDisposed;

        /// <summary>
        /// Gets or sets a value indicating whether the request should be done with the HEAD HTTP method.
        /// </summary>
        public bool HeadOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the request should be done with the OPTIONS HTTP method.
        /// </summary>
        public bool Options { get; set; }

        /// <summary>
        /// Gets the service proxy web response.
        /// </summary>
        public ProxyWebResponse WebResponse { get; private set; }

        /// <summary>
        /// Returns a <see cref="T:System.Net.WebRequest"/> object for the specified resource.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Net.WebRequest"/> object for the specified resource.
        /// </returns>
        /// <param name="address">A <see cref="T:System.Uri"/> that identifies the resource to request.</param>
        protected override WebRequest GetWebRequest(Uri address)
        {
            DateTime modifiedSince = DateTime.MinValue;

            if (Headers[IfModifiedSinceHeader] != null)
            {
                DateTime.TryParse(Headers[IfModifiedSinceHeader], out modifiedSince);
                Headers.Remove(IfModifiedSinceHeader);
            }

            WebRequest request = base.GetWebRequest(address);

            if (request == null)
            {
                return null;
            }

            if (modifiedSince > DateTime.MinValue)
            {
                ((HttpWebRequest) request).IfModifiedSince = modifiedSince;
            }

            if (Options && String.Equals("GET", request.Method, StringComparison.OrdinalIgnoreCase))
            {
                request.Method = "OPTIONS";
            }
            else if (HeadOnly && String.Equals("GET", request.Method, StringComparison.OrdinalIgnoreCase))
            {
                request.Method = "HEAD";
            }

            return request;
        }

        /// <summary>
        /// Returns the <see cref="T:System.Net.WebResponse"/> for the specified <see cref="T:System.Net.WebRequest"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Net.WebResponse"/> containing the response for the specified <see cref="T:System.Net.WebRequest"/>.
        /// </returns>
        /// <param name="request">A <see cref="T:System.Net.WebRequest"/> that is used to obtain the response. </param>
        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);

            if (response == null)
            {
                return null;
            }

            WebResponse = new ProxyWebResponse(response);
            return WebResponse;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Component"/>
        /// and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// true to release both managed and unmanaged resources; false to release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (m_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                var disposableResponse = WebResponse as IDisposable;

                if (disposableResponse != null)
                {
                    disposableResponse.Dispose();
                }
            }

            base.Dispose(disposing);
            m_isDisposed = true;
        }
    }
}
