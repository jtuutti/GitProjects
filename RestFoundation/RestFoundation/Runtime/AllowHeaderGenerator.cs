// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace RestFoundation.Runtime
{
    internal sealed class AllowHeaderGenerator
    {
        private const string AllowHeaderName = "Allow";
        private const string AllowHeaderValueSeparator = ", ";

        private readonly IHttpResponse m_response;

        public AllowHeaderGenerator(IHttpResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            m_response = response;
        }

        public bool TrySetAllowHeaderFromDescriptor(string serviceUrl, IOptionsDescriptor optionsDescriptor)
        {
            IEnumerable<HttpMethod> serviceMethodHttpMethods = optionsDescriptor.ReturnHttpMethodsFor(new Uri(serviceUrl, UriKind.Relative));

            if (serviceMethodHttpMethods == null)
            {
                return false;
            }

            var allowedHttpMethods = new HashSet<HttpMethod>(serviceMethodHttpMethods.Where(m => m != HttpMethod.Options));
            WriteAllowHeader(allowedHttpMethods);

            return true;
        }

        public void SetAllowHeader(string urlTemplate, Type serviceContractType)
        {
            HashSet<HttpMethod> allowedHttpMethods = HttpMethodRegistry.GetHttpMethods(new RouteMetadata(serviceContractType.AssemblyQualifiedName, urlTemplate));
            WriteAllowHeader(allowedHttpMethods);
        }

        private void WriteAllowHeader(HashSet<HttpMethod> allowedHttpMethods)
        {
            if (allowedHttpMethods.Count == 0)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, RestResources.NotFound);
            }

            m_response.SetHeader(AllowHeaderName, String.Join(AllowHeaderValueSeparator, allowedHttpMethods.Select(m => m.ToString().ToUpperInvariant()).OrderBy(m => m)));
        }
    }
}
