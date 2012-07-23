using System.Web;
using NUnit.Framework;
using RestFoundation.Tests.ServiceContracts;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests
{
    public class BrowserDetectorTests
    {
        private IBrowserDetector m_browserDetector;

        [TestFixtureSetUp]
        public void Initialize()
        {
            m_browserDetector = Rest.Active.ServiceLocator.GetService<IBrowserDetector>();
        }

        [Test]
        public void DetectEarlyInternetExplorerBrowserByAcceptHeader()
        {
            const string acceptValue = "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/x-shockwave-flash, application/msword, */*";

            HttpRequestBase request = CreateRequest(acceptValue);
            bool isBrowser = m_browserDetector.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.True);
        }

        [Test]
        public void DetectInternetExplorerBrowserByAcceptHeader()
        {
            const string acceptValue = "text/html, application/xhtml+xml, */*";

            HttpRequestBase request = CreateRequest(acceptValue);
            bool isBrowser = m_browserDetector.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.True);            
        }

        [Test]
        public void DetectMozillaOrWebkitBrowserByAcceptHeader()
        {
            const string acceptValue = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            HttpRequestBase request = CreateRequest(acceptValue);
            bool isBrowser = m_browserDetector.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.True);
        }

        [Test]
        public void DetectEarlyWebkitBrowserByAcceptHeader()
        {
            const string acceptValue = "application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";

            HttpRequestBase request = CreateRequest(acceptValue);
            bool isBrowser = m_browserDetector.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.False); // should return FALSE because XML has a higher priority that HTML
        }

        [Test]
        public void DetectNonBrowserWhenOnlyJsonIsSpecified()
        {
            const string acceptValue = "application/json";

            HttpRequestBase request = CreateRequest(acceptValue);
            bool isBrowser = m_browserDetector.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.False);
        }

        [Test]
        public void DetectNonBrowserWhenOnlyXmlIsSpecified()
        {
            const string acceptValue = "application/xml";

            HttpRequestBase request = CreateRequest(acceptValue);
            bool isBrowser = m_browserDetector.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.False);
        }

        [Test]
        public void DetectNonBrowserWhenOnlyXmlIsSpecifiedWithAlternativeMediaType()
        {
            const string acceptValue = "text/xml";

            HttpRequestBase request = CreateRequest(acceptValue);
            bool isBrowser = m_browserDetector.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.False);
        }

        [Test]
        public void DetectNonBrowserWhenJsonIsSpecifiedWithHighPriority()
        {
            const string acceptValue = "application/json,text/html;q=0.9";

            HttpRequestBase request = CreateRequest(acceptValue);
            bool isBrowser = m_browserDetector.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.False);
        }

        [Test]
        public void DetectNonBrowserWhenXmlIsSpecifiedWithHighPriority()
        {
            const string acceptValue = "application/xml,text/html;q=0.9";

            HttpRequestBase request = CreateRequest(acceptValue);
            bool isBrowser = m_browserDetector.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.False);
        }

        [Test]
        public void DetectNonBrowserWhenXmlIsSpecifiedWithHighPriorityAndAlternativeMediaType()
        {
            const string acceptValue = "text/xml,text/html;q=0.9";

            HttpRequestBase request = CreateRequest(acceptValue);
            bool isBrowser = m_browserDetector.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.False);
        }

        [Test]
        public void DetectBrowserHasAnyTypeSupport()
        {
            const string acceptValue = "*/*";

            HttpRequestBase request = CreateRequest(acceptValue);
            bool isBrowser = m_browserDetector.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.True);
        }

        [Test]
        public void DetectBrowserHasLowHtmlSupport()
        {
            const string acceptValue = "text/plain,image/png;q=0.9,image/jpeg;q=0.5,text/html;charset=utf-8;q=0.1";

            HttpRequestBase request = CreateRequest(acceptValue);
            bool isBrowser = m_browserDetector.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.True);
        }

        [Test]
        public void DetectNonBrowserThatSupportsNoneOfTheRestContentType()
        {
            const string acceptValue = "text/plain,image/png;q=0.9,image/jpeg;q=0.5";

            HttpRequestBase request = CreateRequest(acceptValue);
            bool isBrowser = m_browserDetector.IsBrowserRequest(request);

            Assert.That(isBrowser, Is.False);
        }

        private static HttpRequestBase CreateRequest(string acceptHeaderValue)
        {
            using (var factory = new MockHandlerFactory())
            {
                IRestHandler handler = factory.Create<ITestService>("~/test-service/1", m => m.Get(1));
                Assert.That(handler, Is.Not.Null);
                Assert.That(handler.Context, Is.Not.Null);

                HttpContextBase context = handler.Context.GetHttpContext();
                Assert.That(context, Is.Not.Null);
                Assert.That(context.Request, Is.Not.Null);
                Assert.That(context.Request.Headers, Is.Not.Null);

                context.Request.Headers["Accept"] = acceptHeaderValue;

                return context.Request;
            }
        }
    }
}
