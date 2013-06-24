using System;
using NUnit.Framework;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.ContentNegotiators
{
    [TestFixture]
    public class ContentNegotiatorTests
    {
        private const string JsonMediaType = "application/json";
        private const string XmlMediaType = "application/xml";
        private const string XmlMediaTypeAlt = "text/xml";

        private IContentNegotiator m_contentNegotiator;

        [SetUp]
        public void Initialize()
        {
            m_contentNegotiator = Rest.Configuration.ServiceLocator.GetService<IContentNegotiator>();
        }

        [TearDown]
        public void ShutDown()
        {
            MockContextManager.DestroyContext();
        }

        [Test]
        public void AcceptJsonFromAcceptHeader()
        {
            IHttpRequest request = CreateRequest(JsonMediaType);
            string mediaType = m_contentNegotiator.GetPreferredMediaType(request);

            Assert.That(mediaType, Is.EqualTo(JsonMediaType));
        }

        [Test]
        public void AcceptJsonFromAcceptHeaderWithExtraParameters()
        {
            IHttpRequest request = CreateRequest("text/xml;charset=utf-8;q=0.5,application/json;charset=utf-8");
            string mediaType = m_contentNegotiator.GetPreferredMediaType(request);

            Assert.That(mediaType, Is.EqualTo(JsonMediaType));
        }

        [Test]
        public void AcceptXmlFromAcceptHeader()
        {
            IHttpRequest request = CreateRequest(XmlMediaType);
            string mediaType = m_contentNegotiator.GetPreferredMediaType(request);

            Assert.That(mediaType, Is.EqualTo(XmlMediaType));
        }

        [Test]
        public void AcceptXmlFromAcceptHeaderWithExtraParameters()
        {
            IHttpRequest request = CreateRequest("application/xml;charset=utf-8,application/json;charset=utf-8;q=0.5");
            string mediaType = m_contentNegotiator.GetPreferredMediaType(request);

            Assert.That(mediaType, Is.EqualTo(XmlMediaType));
        }

        [Test]
        public void AcceptXmlAltFromAcceptHeader()
        {
            IHttpRequest request = CreateRequest(XmlMediaTypeAlt);
            string mediaType = m_contentNegotiator.GetPreferredMediaType(request);

            Assert.That(mediaType, Is.EqualTo(XmlMediaTypeAlt));
        }

        [Test]
        public void AcceptXmlAltFromAcceptHeaderWithExtraParameters()
        {
            IHttpRequest request = CreateRequest("application/json;charset=utf-8;q=0.5,text/xml;charset=utf-8");
            string mediaType = m_contentNegotiator.GetPreferredMediaType(request);

            Assert.That(mediaType, Is.EqualTo(XmlMediaTypeAlt));
        }

        [Test]
        public void AcceptJsonFromContentType()
        {
            IHttpRequest request = CreateRequest(contentTypeHeaderValue: "application/json;charset=utf-8");
            string mediaType = m_contentNegotiator.GetPreferredMediaType(request);

            Assert.That(mediaType, Is.EqualTo(JsonMediaType));
        }

        [Test]
        public void AcceptXmlFromContentType()
        {
            IHttpRequest request = CreateRequest(contentTypeHeaderValue: "application/xml;charset=utf-8");
            string mediaType = m_contentNegotiator.GetPreferredMediaType(request);

            Assert.That(mediaType, Is.EqualTo(XmlMediaType));
        }

        [Test]
        public void AcceptJsonFromJsonRequest()
        {
            IHttpRequest request = CreateRequest(isAjax: true);
            string mediaType = m_contentNegotiator.GetPreferredMediaType(request);

            Assert.That(mediaType, Is.EqualTo(JsonMediaType));
        }

        [Test]
        public void AcceptJsonFromQueryStringOverride()
        {
            IHttpRequest request = CreateRequest(XmlMediaType, acceptOverrideValue: JsonMediaType, contentTypeHeaderValue: JsonMediaType);
            string mediaType = m_contentNegotiator.GetPreferredMediaType(request);

            Assert.That(mediaType, Is.EqualTo(JsonMediaType));
        }

        [Test]
        public void AcceptXmlFromQueryStringOverride()
        {
            IHttpRequest request = CreateRequest(JsonMediaType, acceptOverrideValue: XmlMediaType, contentTypeHeaderValue: XmlMediaType);
            string mediaType = m_contentNegotiator.GetPreferredMediaType(request);

            Assert.That(mediaType, Is.EqualTo(XmlMediaType));
        }

        [Test]
        public void NoAcceptTypesProvided()
        {
            IHttpRequest request = CreateRequest();
            string mediaType = m_contentNegotiator.GetPreferredMediaType(request);

            Assert.That(mediaType, Is.Null);
        }

        [Test]
        public void DetectBrowserByUserAgent_Chrome()
        {
            IHttpRequest request = CreateRequest(userAgentValue: "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/525.13 (KHTML, like Gecko) Chrome/0.2.149.29 Safari/525.13");
            bool isBrowser = m_contentNegotiator.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.True);
        }

        [Test]
        public void DetectBrowserByUserAgent_Firefox()
        {
            IHttpRequest request = CreateRequest(userAgentValue: "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.11) Gecko/20071127 Firefox/2.0.0.11");
            bool isBrowser = m_contentNegotiator.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.True);
        }

        [Test]
        public void DetectBrowserByUserAgent_InternetExplorer()
        {
            IHttpRequest request = CreateRequest(userAgentValue: "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)");
            bool isBrowser = m_contentNegotiator.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.True);
        }

        [Test]
        public void DetectBrowserByUserAgent_Opera()
        {
            IHttpRequest request = CreateRequest(userAgentValue: "Opera/8.0 (Windows NT 5.1; U; en)");
            bool isBrowser = m_contentNegotiator.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.True);
        }

        [Test]
        public void DetectBrowserByUserAgent_Safari()
        {
            IHttpRequest request = CreateRequest(userAgentValue: "Mozilla/5.0 (Macintosh; U; PPC Mac OS X; en) AppleWebKit/124 (KHTML, like Gecko) Safari/125.1");
            bool isBrowser = m_contentNegotiator.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.True);
        }

        [Test]
        public void DetectBrowserByUserAgent_RestFoundationProxy()
        {
            IHttpRequest request = CreateRequest(userAgentValue: "Rest Foundation Proxy");
            bool isBrowser = m_contentNegotiator.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.False);
        }

        [Test]
        public void DetectBrowserByUserAgent_ServiceCall()
        {
            IHttpRequest request = CreateRequest();
            bool isBrowser = m_contentNegotiator.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.False);
        }

        private static IHttpRequest CreateRequest(string acceptHeaderValue = null,
                                                  string contentTypeHeaderValue = null,
                                                  string acceptOverrideValue = null,
                                                  bool isAjax = false,
                                                  string userAgentValue = null)
        {
            var context = MockContextManager.GenerateContext("~/test-service/new", HttpMethod.Post);

            if (!String.IsNullOrEmpty(acceptHeaderValue))
            {
                MockContextManager.SetHeader("Accept", acceptHeaderValue);
            }

            if (!String.IsNullOrEmpty(contentTypeHeaderValue))
            {
                MockContextManager.SetHeader("Content-Type", contentTypeHeaderValue);
            }

            if (isAjax)
            {
                MockContextManager.SetHeader("X-Requested-With", "XMLHttpRequest");
            }

            if (!String.IsNullOrEmpty(acceptOverrideValue))
            {
                MockContextManager.SetQuery("X-Accept-Override", acceptOverrideValue);
            }

            if (!String.IsNullOrEmpty(userAgentValue))
            {
                MockContextManager.SetServerVariable("HTTP_USER_AGENT", userAgentValue);
            }

            return context.Request;
        }
    }
}
