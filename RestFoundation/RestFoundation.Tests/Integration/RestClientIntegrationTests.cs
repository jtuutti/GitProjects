using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using RestFoundation.Client;
using RestTestContracts.Resources;

namespace RestFoundation.Tests.Integration
{
    [TestFixture]
    public class RestClientIntegrationTests
    {
        private const string PersonXmlNamespace = "urn:com.rest-test.resources";

        private readonly string integrationServiceUrl = ConfigurationManager.AppSettings["LocalIntegrationServiceUrl"];

        [Test]
        public async Task TestGetMethod_HttpOptions()
        {
            IRestClient client = RestClientFactory.Create();

            IReadOnlyList<HttpMethod> httpMethods = await client.OptionsAsync(new Uri(integrationServiceUrl + "/home/index/1"));
            Assert.That(httpMethods, Is.Not.Null);
            Assert.That(httpMethods.Count, Is.GreaterThan(0));
            Assert.That(httpMethods.Any(m => m == HttpMethod.Post), Is.False);
        }

        [Test]
        public async Task TestGetAllMethod_HttpHead_Json()
        {
            IRestClient client = RestClientFactory.Create();

            WebHeaderCollection responseHeaders = await client.HeadAsync(new Uri(integrationServiceUrl + "/home/index/all"), RestResourceType.Json);
            Assert.That(responseHeaders, Is.Not.Null);
            Assert.That(responseHeaders.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task TestGetAllMethod_HttpHead_Xml()
        {
            IRestClient client = RestClientFactory.Create();

            WebHeaderCollection responseHeaders = await client.HeadAsync(new Uri(integrationServiceUrl + "/home/index/all"), RestResourceType.Xml);
            Assert.That(responseHeaders, Is.Not.Null);
            Assert.That(responseHeaders.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task TestGetAllMethod_HttpGet_Json()
        {
            IRestClient client = RestClientFactory.Create();

            RestResource<Person[]> resource = await client.GetAsync<Person[]>(new Uri(integrationServiceUrl + "/home/index/all"), RestResourceType.Json);
            Assert.That(resource, Is.Not.Null);
            Assert.That(resource.Body, Is.Not.Null);
            Assert.That(resource.Body.Length, Is.GreaterThan(0));

            foreach (Person person in resource.Body)
            {
                Assert.That(person, Is.Not.Null);
                Assert.That(person.Age, Is.GreaterThan(0));
                Assert.That(person.Name, Is.Not.Null);
                Assert.That(person.Name, Is.Not.Empty);
            }
        }

        [Test]
        public async Task TestGetAllMethod_HttpGet_Xml()
        {
            IRestClient client = RestClientFactory.Create();

            RestResource<Person[]> resource = await client.GetAsync<Person[]>(new Uri(integrationServiceUrl + "/home/index/all"), RestResourceType.Xml, null, PersonXmlNamespace);
            Assert.That(resource, Is.Not.Null);
            Assert.That(resource.Body, Is.Not.Null);
            Assert.That(resource.Body.Length, Is.GreaterThan(0));

            foreach (Person person in resource.Body)
            {
                Assert.That(person, Is.Not.Null);
                Assert.That(person.Age, Is.GreaterThan(0));
                Assert.That(person.Name, Is.Not.Null);
                Assert.That(person.Name, Is.Not.Empty);
            }
        }

        [Test]
        public async Task TestGetPostMethod_HttpPost_Json()
        {
            IRestClient client = RestClientFactory.Create();

            var inputResource = new RestResource<Person>(RestResourceType.Json)
            {
                Body = new Person
                {
                    Name = "Joe Bloe",
                    Age = 41
                }
            };

            var outputResource = await client.PostAsync<Person, Person>(new Uri(integrationServiceUrl + "/home/index"), inputResource);
            Assert.That(outputResource, Is.Not.Null);
            Assert.That(outputResource.Body, Is.Not.Null);
            Assert.That(outputResource.Body.Name, Is.EqualTo(inputResource.Body.Name));
            Assert.That(outputResource.Body.Age, Is.EqualTo(inputResource.Body.Age));
            Assert.That(outputResource.Body.Values, Is.Not.Null);
        }

        [Test]
        public async Task TestGetPostMethod_HttpPost_Xml()
        {
            IRestClient client = RestClientFactory.Create();

            var inputResource = new RestResource<Person>(RestResourceType.Xml)
            {
                Body = new Person
                {
                    Name = "Joe Bloe",
                    Age = 41
                },
                XmlNamespace = PersonXmlNamespace
            };

            var outputResource = await client.PostAsync<Person, Person>(new Uri(integrationServiceUrl + "/home/index"), inputResource);
            Assert.That(outputResource, Is.Not.Null);
            Assert.That(outputResource.Body, Is.Not.Null);
            Assert.That(outputResource.Body.Name, Is.EqualTo(inputResource.Body.Name));
            Assert.That(outputResource.Body.Age, Is.EqualTo(inputResource.Body.Age));
            Assert.That(outputResource.Body.Values, Is.Not.Null);
        }
    }
}
