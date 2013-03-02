using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using NUnit.Framework;
using RestFoundation.Client;
using RestTestContracts.Resources;

namespace RestFoundation.Tests.Integration
{
    [TestFixture]
    public class RestClientIntegrationTests
    {
        private const string PersonXmlNamespace = "urn:com.rest-test.resources";

        private readonly string integrationServiceUri = ConfigurationManager.AppSettings["LocalIntegrationServiceUrl"];

        [Test]
        public void TestGetMethod_HttpOptions()
        {
            IRestClient client = RestClientFactory.Create();

            var serviceUri = new Uri(integrationServiceUri + "/home/index/1");
            IReadOnlyList<HttpMethod> httpMethods = client.OptionsAsync(serviceUri).Result;
            Assert.That(httpMethods, Is.Not.Null);
            Assert.That(httpMethods.Count, Is.GreaterThan(0));
            Assert.That(httpMethods.Any(m => m == HttpMethod.Post), Is.False);
        }

        [Test]
        public void TestGetAllMethod_HttpHead_Json()
        {
            IRestClient client = RestClientFactory.Create();

            var serviceUri = new Uri(integrationServiceUri + "/home/index/all");
            WebHeaderCollection responseHeaders = client.HeadAsync(serviceUri, RestResourceType.Json).Result;
            Assert.That(responseHeaders, Is.Not.Null);
            Assert.That(responseHeaders.Count, Is.GreaterThan(0));
        }

        [Test]
        public void TestGetAllMethod_HttpHead_Xml()
        {
            IRestClient client = RestClientFactory.Create();

            var serviceUri = new Uri(integrationServiceUri + "/home/index/all");
            WebHeaderCollection responseHeaders = client.HeadAsync(serviceUri, RestResourceType.Xml).Result;
            Assert.That(responseHeaders, Is.Not.Null);
            Assert.That(responseHeaders.Count, Is.GreaterThan(0));
        }

        [Test]
        public void TestGetAllMethod_HttpGet_Json()
        {
            IRestClient client = RestClientFactory.Create();

            var serviceUri = new Uri(integrationServiceUri + "/home/index/all");
            RestResource<Person[]> resource = client.GetAsync<Person[]>(serviceUri, RestResourceType.Json).Result;
            Assert.That(client.LastStatusCode, Is.EqualTo(HttpStatusCode.OK));
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
        public void TestGetAllMethod_HttpGet_Xml()
        {
            IRestClient client = RestClientFactory.Create();

            var serviceUri = new Uri(integrationServiceUri + "/home/index/all");
            RestResource<Person[]> resource = client.GetAsync<Person[]>(serviceUri, RestResourceType.Xml, null, PersonXmlNamespace).Result;
            Assert.That(client.LastStatusCode, Is.EqualTo(HttpStatusCode.OK));
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
        public void TestPostMethod_Json()
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

            var serviceUri = new Uri(integrationServiceUri + "/home/index");
            var outputResource = client.PostAsync<Person, Person>(serviceUri, inputResource).Result;
            Assert.That(client.LastStatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(outputResource, Is.Not.Null);
            Assert.That(outputResource.Body, Is.Not.Null);
            Assert.That(outputResource.Body.Name, Is.EqualTo(inputResource.Body.Name));
            Assert.That(outputResource.Body.Age, Is.EqualTo(inputResource.Body.Age));
            Assert.That(outputResource.Body.Values, Is.Not.Null);
        }

        [Test]
        public void TestPostMethod_Xml()
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

            var serviceUri = new Uri(integrationServiceUri + "/home/index");
            var outputResource = client.PostAsync<Person, Person>(serviceUri, inputResource).Result;
            Assert.That(client.LastStatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(outputResource, Is.Not.Null);
            Assert.That(outputResource.Body, Is.Not.Null);
            Assert.That(outputResource.Body.Name, Is.EqualTo(inputResource.Body.Name));
            Assert.That(outputResource.Body.Age, Is.EqualTo(inputResource.Body.Age));
            Assert.That(outputResource.Body.Values, Is.Not.Null);
        }

        [Test]
        public void TestPutMethod_Json()
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

            var serviceUri = new Uri(integrationServiceUri + "/home/index/1");
            var outputResource = client.PutAsync<Person, Person>(serviceUri, inputResource).Result;
            Assert.That(client.LastStatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(outputResource, Is.Not.Null);
            Assert.That(outputResource.Body, Is.Not.Null);
            Assert.That(outputResource.Body.Name, Is.EqualTo(inputResource.Body.Name));
            Assert.That(outputResource.Body.Age, Is.EqualTo(inputResource.Body.Age));
            Assert.That(outputResource.Body.Values, Is.Not.Null);
        }

        [Test]
        public void TestPutMethod_Xml()
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

            var serviceUri = new Uri(integrationServiceUri + "/home/index/1");
            var outputResource = client.PutAsync<Person, Person>(serviceUri, inputResource).Result;
            Assert.That(client.LastStatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(outputResource, Is.Not.Null);
            Assert.That(outputResource.Body, Is.Not.Null);
            Assert.That(outputResource.Body.Name, Is.EqualTo(inputResource.Body.Name));
            Assert.That(outputResource.Body.Age, Is.EqualTo(inputResource.Body.Age));
            Assert.That(outputResource.Body.Values, Is.Not.Null);
        }

        [Test]
        public void TestPatchMethod_Json()
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

            var serviceUri = new Uri(integrationServiceUri + "/home/index/1");
            var outputResource = client.PatchAsync<Person, Person>(serviceUri, inputResource).Result;
            Assert.That(client.LastStatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(outputResource, Is.Not.Null);
            Assert.That(outputResource.Body, Is.Not.Null);
            Assert.That(outputResource.Body.Name, Is.EqualTo(inputResource.Body.Name));
            Assert.That(outputResource.Body.Age, Is.EqualTo(inputResource.Body.Age));
            Assert.That(outputResource.Body.Values, Is.Not.Null);
        }

        [Test]
        public void TestPatchMethod_Xml()
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

            var serviceUri = new Uri(integrationServiceUri + "/home/index/1");
            var outputResource = client.PatchAsync<Person, Person>(serviceUri, inputResource).Result;
            Assert.That(client.LastStatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(outputResource, Is.Not.Null);
            Assert.That(outputResource.Body, Is.Not.Null);
            Assert.That(outputResource.Body.Name, Is.EqualTo(inputResource.Body.Name));
            Assert.That(outputResource.Body.Age, Is.EqualTo(inputResource.Body.Age));
            Assert.That(outputResource.Body.Values, Is.Not.Null);
        }

        [Test]
        public void TestDeleteMethod()
        {
            IRestClient client = RestClientFactory.Create();

            var serviceUri = new Uri(integrationServiceUri + "/home/index/1");
            var outputResponse = client.DeleteAsync(serviceUri).Result;
            Assert.That(client.LastStatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            Assert.That(outputResponse, Is.Not.Null);
            Assert.That(outputResponse.Count, Is.GreaterThan(0));
        }

        [Test]
        [ExpectedException(typeof(HttpException))]
        public async Task TestBadUri()
        {
            IRestClient client = RestClientFactory.Create();

            try
            {
                var serviceUri = new Uri(integrationServiceUri + "/home/index/bad-url");
                var outputResponse = await client.GetAsync<Person>(serviceUri, RestResourceType.Json);
                Assert.That(outputResponse, Is.Not.Null);
            }
            finally
            {
                Assert.That(client.LastStatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            }
        }

        [Test]
        [ExpectedException(typeof(HttpException))]
        public async Task TestBadHttpMethod()
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

            var serviceUri = new Uri(integrationServiceUri + "/home/index/1");

            try
            {
                var outputResource = await client.PostAsync<Person, Person>(serviceUri, inputResource);
                Assert.That(outputResource, Is.Not.Null);
            }
            finally
            {
                Assert.That(client.LastStatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));
            }
        }
    }
}
