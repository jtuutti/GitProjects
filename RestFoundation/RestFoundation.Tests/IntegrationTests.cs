using System;
using System.Net;
using NUnit.Framework;
using RestFoundation.Client;

namespace RestFoundation.Tests
{
    [TestFixture]
    public class IntegrationTests
    {
        [Test]
        public void ConnectToRestTest_GetAll()
        {
            var serviceUrl = new Uri("https://localhost:8443/rest/async/index/all");

            IRestClient client = RestClientFactory.Create();
            client.AllowSelfSignedCertificates = true;
            client.AuthenticationType = "Digest";
            client.Credentials = new NetworkCredential("admin", "Rest", "https://localhost:8443/rest");

            RestResource<dynamic> result = client.Get<dynamic>(serviceUrl, RestResourceType.Xml);

        }
    }
}
