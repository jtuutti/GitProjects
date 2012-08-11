using System;
using System.Web;
using NUnit.Framework;
using RestFoundation.Runtime.Handlers;
using RestFoundation.Tests.Implementation.ServiceContracts;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.ContentNegotiators
{
    [TestFixture]
    public class ContentNegotiatorTests
    {
        private const string JsonMediaType = "application/json";
        private const string XmlMediaType = "application/xml";
        private const string XmlMediaTypeAlt = "text/xml";

        private MockHandlerFactory m_factory;
        private IContentNegotiator m_contentNegotiator;

        [SetUp]
        public void Initialize()
        {
            m_contentNegotiator = Rest.Active.ServiceLocator.GetService<IContentNegotiator>();
            m_factory = new MockHandlerFactory();
        }

        [TearDown]
        public void ShutDown()
        {
            m_factory.Dispose();
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
        public void DetectEarlyInternetExplorerBrowserByAcceptHeader()
        {
            const string acceptValue = "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/x-shockwave-flash, application/msword, */*";

            IHttpRequest request = CreateRequestBase(acceptValue);
            bool isBrowser = m_contentNegotiator.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.True);
        }

        [Test]
        public void DetectInternetExplorerBrowserByAcceptHeader()
        {
            const string acceptValue = "text/html, application/xhtml+xml, */*";

            IHttpRequest request = CreateRequestBase(acceptValue);
            bool isBrowser = m_contentNegotiator.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.True);            
        }

        [Test]
        public void DetectMozillaOrWebkitBrowserByAcceptHeader()
        {
            const string acceptValue = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            IHttpRequest request = CreateRequestBase(acceptValue);
            bool isBrowser = m_contentNegotiator.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.True);
        }

        [Test]
        public void DetectEarlyWebkitBrowserByAcceptHeader()
        {
            const string acceptValue = "application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";

            IHttpRequest request = CreateRequestBase(acceptValue);
            bool isBrowser = m_contentNegotiator.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.False); // should return FALSE because XML has a higher priority that HTML
        }

        [Test]
        public void DetectNonBrowserWhenOnlyJsonIsSpecified()
        {
            const string acceptValue = "application/json";

            IHttpRequest request = CreateRequestBase(acceptValue);
            bool isBrowser = m_contentNegotiator.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.False);
        }

        [Test]
        public void DetectNonBrowserWhenOnlyXmlIsSpecified()
        {
            const string acceptValue = "application/xml";

            IHttpRequest request = CreateRequestBase(acceptValue);
            bool isBrowser = m_contentNegotiator.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.False);
        }

        [Test]
        public void DetectNonBrowserWhenOnlyXmlIsSpecifiedWithAlternativeMediaType()
        {
            const string acceptValue = "text/xml";

            IHttpRequest request = CreateRequestBase(acceptValue);
            bool isBrowser = m_contentNegotiator.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.False);
        }

        [Test]
        public void DetectNonBrowserWhenJsonIsSpecifiedWithHighPriority()
        {
            const string acceptValue = "application/json,text/html;q=0.9";

            IHttpRequest request = CreateRequestBase(acceptValue);
            bool isBrowser = m_contentNegotiator.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.False);
        }

        [Test]
        public void DetectNonBrowserWhenXmlIsSpecifiedWithHighPriority()
        {
            const string acceptValue = "application/xml,text/html;q=0.9";

            IHttpRequest request = CreateRequestBase(acceptValue);
            bool isBrowser = m_contentNegotiator.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.False);
        }

        [Test]
        public void DetectNonBrowserWhenXmlIsSpecifiedWithHighPriorityAndAlternativeMediaType()
        {
            const string acceptValue = "text/xml,text/html;q=0.9";

            IHttpRequest request = CreateRequestBase(acceptValue);
            bool isBrowser = m_contentNegotiator.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.False);
        }

        [Test]
        public void DetectBrowserHasAnyTypeSupport()
        {
            const string acceptValue = "*/*";

            IHttpRequest request = CreateRequestBase(acceptValue);
            bool isBrowser = m_contentNegotiator.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.True);
        }

        [Test]
        public void DetectBrowserHasLowHtmlSupport()
        {
            const string acceptValue = "text/plain,image/png;q=0.9,image/jpeg;q=0.5,text/html;charset=utf-8;q=0.1";

            IHttpRequest request = CreateRequestBase(acceptValue);
            bool isBrowser = m_contentNegotiator.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.True);
        }

        [Test]
        public void DetectNonBrowserThatSupportsNoneOfTheRestContentType()
        {
            const string acceptValue = "text/plain,image/png;q=0.9,image/jpeg;q=0.5";

            IHttpRequest request = CreateRequestBase(acceptValue);
            bool isBrowser = m_contentNegotiator.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.False);
        }

        private IHttpRequest CreateRequest(string acceptHeaderValue = null, string contentTypeHeaderValue = null, string acceptOverrideValue = null, bool isAjax = false)
        {
            IRestHandler handler = m_factory.Create<ITestService>("~/test-service/new", m => m.Post(null));
            Assert.That(handler, Is.Not.Null);
            Assert.That(handler.Context, Is.Not.Null);

            HttpContextBase context = handler.Context.GetHttpContext();
            Assert.That(context, Is.Not.Null);
            Assert.That(context.Request, Is.Not.Null);
            Assert.That(context.Request.Headers, Is.Not.Null);

            if (!String.IsNullOrEmpty(acceptHeaderValue))
            {
                context.Request.Headers["Accept"] = acceptHeaderValue;
            }

            if (!String.IsNullOrEmpty(contentTypeHeaderValue))
            {
                context.Request.Headers["Content-Type"] = contentTypeHeaderValue;
            }

            if (!String.IsNullOrEmpty(acceptOverrideValue))
            {
                context.Request.QueryString["X-Accept-Override"] = acceptOverrideValue;
            }

            if (isAjax)
            {
                context.Request.Headers["X-Requested-With"] = "XMLHttpRequest";
            }

            return handler.Context.Request;
        }

        private IHttpRequest CreateRequestBase(string acceptHeaderValue)
        {
            IRestHandler handler = m_factory.Create<ITestService>("~/test-service/1", m => m.Get(1));
            Assert.That(handler, Is.Not.Null);
            Assert.That(handler.Context, Is.Not.Null);

            HttpContextBase context = handler.Context.GetHttpContext();
            Assert.That(context, Is.Not.Null);
            Assert.That(context.Request, Is.Not.Null);
            Assert.That(context.Request.Headers, Is.Not.Null);

            context.Request.Headers["Accept"] = acceptHeaderValue;

            return handler.Context.Request;
        }
    }
}
