using System;
using System.Net;
using System.Web;
using System.Web.Routing;
using NUnit.Framework;
using RestFoundation.Runtime;
using RestFoundation.Test;
using RestFoundation.Tests.ServiceContracts;
using StructureMap;

namespace RestFoundation.Tests
{
    [TestFixture]
    public class RouteHandlerTests
    {
        [Test]
        public void GetMethodWithSyncHandler()
        {
            var handler = ObjectFactory.GetInstance<RestHandler>();
            Assert.That(handler, Is.Not.Null);

            RequestContext requestContext = RequestContextMockFactory.Create(typeof(ITestService), SetUpFixture.RelativeUrl, "{id}", "GET");
            Assert.That(requestContext, Is.Not.Null);

            IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
            Assert.That(httpHandler, Is.Not.Null);
            Assert.That(httpHandler, Is.InstanceOf<RestHandler>());

            var request = ObjectFactory.GetInstance<IHttpRequest>();
            Assert.That(request, Is.Not.Null);
            request.SetRouteValues(new { id = 1 });

            ProcessRequest(handler);

            var response = ObjectFactory.GetInstance<IHttpResponse>();
            Assert.That(response, Is.Not.Null);
            Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void GetMethodWithAsyncHandler()
        {
            var handler = ObjectFactory.GetInstance<RestAsyncHandler>();
            Assert.That(handler, Is.Not.Null);

            RequestContext requestContext = RequestContextMockFactory.Create(typeof(ITestService), SetUpFixture.RelativeUrl, "{id}", "GET");
            Assert.That(requestContext, Is.Not.Null);

            IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
            Assert.That(httpHandler, Is.Not.Null);
            Assert.That(httpHandler, Is.InstanceOf<RestAsyncHandler>());

            var request = ObjectFactory.GetInstance<IHttpRequest>();
            Assert.That(request, Is.Not.Null);
            request.SetRouteValues(new { id = 1 });

            ProcessRequest(handler);

            var response = ObjectFactory.GetInstance<IHttpResponse>();
            Assert.That(response, Is.Not.Null);
            Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
        }

        [Test, ExpectedException(typeof(HttpResponseException))]
        public void GetMethodWithSyncHandler_ShouldFailDueToNegativeIdValue()
        {
            var handler = ObjectFactory.GetInstance<RestHandler>();
            Assert.That(handler, Is.Not.Null);

            RequestContext requestContext = RequestContextMockFactory.Create(typeof(ITestService), SetUpFixture.RelativeUrl, "{id}", "GET");
            Assert.That(requestContext, Is.Not.Null);

            IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
            Assert.That(httpHandler, Is.Not.Null);
            Assert.That(httpHandler, Is.InstanceOf<RestHandler>());

            var request = ObjectFactory.GetInstance<IHttpRequest>();
            Assert.That(request, Is.Not.Null);
            request.SetRouteValues(new { id = -1 });

            ProcessRequest(handler);

            var response = ObjectFactory.GetInstance<IHttpResponse>();
            Assert.That(response, Is.Not.Null);
            Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
        }

        [Test, ExpectedException(typeof(NotSupportedException))]
        public void GetMethodWithAsyncHandler_ShouldFailDueToCallingSynchronousProcessRequest()
        {
            var handler = ObjectFactory.GetInstance<RestAsyncHandler>();
            Assert.That(handler, Is.Not.Null);
            
            RequestContext requestContext = RequestContextMockFactory.Create(typeof(ITestService), SetUpFixture.RelativeUrl, "{id}", "GET");
            Assert.That(requestContext, Is.Not.Null);

            IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
            Assert.That(httpHandler, Is.Not.Null);
            Assert.That(httpHandler, Is.InstanceOf<RestAsyncHandler>());

            var request = ObjectFactory.GetInstance<IHttpRequest>();
            Assert.That(request, Is.Not.Null);
            request.SetRouteValues(new { id = 1 });

            handler.ProcessRequest(null);

            var response = ObjectFactory.GetInstance<IHttpResponse>();
            Assert.That(response, Is.Not.Null);
            Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void HeadMethodWithSyncHandler()
        {
            var handler = ObjectFactory.GetInstance<RestHandler>();
            Assert.That(handler, Is.Not.Null);
            
            RequestContext requestContext = RequestContextMockFactory.Create(typeof(ITestService), SetUpFixture.RelativeUrl, "all/{orderBy}", "HEAD");
            Assert.That(requestContext, Is.Not.Null);

            IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
            Assert.That(httpHandler, Is.Not.Null);
            Assert.That(httpHandler, Is.InstanceOf<RestHandler>());

            var request = ObjectFactory.GetInstance<IHttpRequest>();
            Assert.That(request, Is.Not.Null);
            request.SetRouteValues(new { orderBy = "Name" });

            ProcessRequest(handler);

            var response = ObjectFactory.GetInstance<IHttpResponse>();
            Assert.That(response, Is.Not.Null);
            Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void HeadMethodWithAsyncHandler()
        {
            var handler = ObjectFactory.GetInstance<RestAsyncHandler>();
            Assert.That(handler, Is.Not.Null);
            
            RequestContext requestContext = RequestContextMockFactory.Create(typeof(ITestService), SetUpFixture.RelativeUrl, "all/{orderBy}", "HEAD");
            Assert.That(requestContext, Is.Not.Null);

            var request = ObjectFactory.GetInstance<IHttpRequest>();
            Assert.That(request, Is.Not.Null);
            request.SetRouteValues(new { orderBy = "Name" });

            IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
            Assert.That(httpHandler, Is.Not.Null);
            Assert.That(httpHandler, Is.InstanceOf<RestAsyncHandler>());

            ProcessRequest(handler);

            var response = ObjectFactory.GetInstance<IHttpResponse>();
            Assert.That(response, Is.Not.Null);
            Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void PostMethodWithSyncHandler()
        {
            var handler = ObjectFactory.GetInstance<RestHandler>();
            Assert.That(handler, Is.Not.Null);
            
            RequestContext requestContext = RequestContextMockFactory.Create(typeof(ITestService), SetUpFixture.RelativeUrl, "new", "POST");
            Assert.That(requestContext, Is.Not.Null);

            IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
            Assert.That(httpHandler, Is.Not.Null);
            Assert.That(httpHandler, Is.InstanceOf<RestHandler>());

            var request = ObjectFactory.GetInstance<IHttpRequest>();
            Assert.That(request, Is.Not.Null);

            ProcessRequest(handler);

            var response = ObjectFactory.GetInstance<IHttpResponse>();
            Assert.That(response, Is.Not.Null);
            Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.Created));
            Assert.That(response.GetHeader("Location"), Is.EqualTo(String.Concat(request.Url.ServiceUrl, "/1")));
        }

        [Test, ExpectedException(typeof(HttpResponseException))]
        public void PostMethodWithSyncHandler_ShouldFailDueToMismatchedUrlTemplate()
        {
            var handler = ObjectFactory.GetInstance<RestHandler>();
            Assert.That(handler, Is.Not.Null);
            
            RequestContext requestContext = RequestContextMockFactory.Create(typeof(ITestService), SetUpFixture.RelativeUrl, "{id}", "POST");
            Assert.That(requestContext, Is.Not.Null);

            IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
            Assert.That(httpHandler, Is.Not.Null);
            Assert.That(httpHandler, Is.InstanceOf<RestHandler>());

            var request = ObjectFactory.GetInstance<IHttpRequest>();
            Assert.That(request, Is.Not.Null);

            ProcessRequest(handler);

            var response = ObjectFactory.GetInstance<IHttpResponse>();
            Assert.That(response, Is.Not.Null);
            Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.Created));
            Assert.That(response.GetHeader("Location"), Is.EqualTo(String.Concat(request.Url, "/1")));
        }

        [Test]
        public void PostMethodWithAsyncHandler()
        {
            var handler = ObjectFactory.GetInstance<RestAsyncHandler>();
            Assert.That(handler, Is.Not.Null);
            
            RequestContext requestContext = RequestContextMockFactory.Create(typeof(ITestService), SetUpFixture.RelativeUrl, "new", "POST");
            Assert.That(requestContext, Is.Not.Null);

            var request = ObjectFactory.GetInstance<IHttpRequest>();
            Assert.That(request, Is.Not.Null);

            IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
            Assert.That(httpHandler, Is.Not.Null);
            Assert.That(httpHandler, Is.InstanceOf<RestAsyncHandler>());

            ProcessRequest(handler);

            var response = ObjectFactory.GetInstance<IHttpResponse>();
            Assert.That(response, Is.Not.Null);
            Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.Created));
            Assert.That(response.GetHeader("Location"), Is.EqualTo(String.Concat(request.Url, "/1")));
        }

        [Test]
        public void PutMethodWithSyncHandler()
        {
            var handler = ObjectFactory.GetInstance<RestHandler>();
            Assert.That(handler, Is.Not.Null);
            
            RequestContext requestContext = RequestContextMockFactory.Create(typeof(ITestService), SetUpFixture.RelativeUrl, "{id}", "PUT");
            Assert.That(requestContext, Is.Not.Null);

            IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
            Assert.That(httpHandler, Is.Not.Null);
            Assert.That(httpHandler, Is.InstanceOf<RestHandler>());

            var request = ObjectFactory.GetInstance<IHttpRequest>();
            Assert.That(request, Is.Not.Null);
            request.SetRouteValues(new { id = 1 });

            ProcessRequest(handler);

            var response = ObjectFactory.GetInstance<IHttpResponse>();
            Assert.That(response, Is.Not.Null);
            Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void PutMethodWithAsyncHandler()
        {
            var handler = ObjectFactory.GetInstance<RestAsyncHandler>();
            Assert.That(handler, Is.Not.Null);
            
            RequestContext requestContext = RequestContextMockFactory.Create(typeof(ITestService), SetUpFixture.RelativeUrl, "{id}", "PUT");
            Assert.That(requestContext, Is.Not.Null);

            IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
            Assert.That(httpHandler, Is.Not.Null);
            Assert.That(httpHandler, Is.InstanceOf<RestAsyncHandler>());

            var request = ObjectFactory.GetInstance<IHttpRequest>();
            Assert.That(request, Is.Not.Null);
            request.SetRouteValues(new { id = 1 });

            ProcessRequest(handler);

            var response = ObjectFactory.GetInstance<IHttpResponse>();
            Assert.That(response, Is.Not.Null);
            Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void PatchMethodWithSyncHandler()
        {
            var handler = ObjectFactory.GetInstance<RestHandler>();
            Assert.That(handler, Is.Not.Null);
            
            RequestContext requestContext = RequestContextMockFactory.Create(typeof(ITestService), SetUpFixture.RelativeUrl, "{id}", "PATCH");
            Assert.That(requestContext, Is.Not.Null);

            IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
            Assert.That(httpHandler, Is.Not.Null);
            Assert.That(httpHandler, Is.InstanceOf<RestHandler>());

            var request = ObjectFactory.GetInstance<IHttpRequest>();
            Assert.That(request, Is.Not.Null);
            request.SetRouteValues(new { id = 1 });

            ProcessRequest(handler);

            var response = ObjectFactory.GetInstance<IHttpResponse>();
            Assert.That(response, Is.Not.Null);
            Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void PatchMethodWithAsyncHandler()
        {
            var handler = ObjectFactory.GetInstance<RestAsyncHandler>();
            Assert.That(handler, Is.Not.Null);
            
            RequestContext requestContext = RequestContextMockFactory.Create(typeof(ITestService), SetUpFixture.RelativeUrl, "{id}", "PATCH");
            Assert.That(requestContext, Is.Not.Null);

            IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
            Assert.That(httpHandler, Is.Not.Null);
            Assert.That(httpHandler, Is.InstanceOf<RestAsyncHandler>());

            var request = ObjectFactory.GetInstance<IHttpRequest>();
            Assert.That(request, Is.Not.Null);
            request.SetRouteValues(new { id = 1 });

            ProcessRequest(handler);

            var response = ObjectFactory.GetInstance<IHttpResponse>();
            Assert.That(response, Is.Not.Null);
            Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void DeleteMethodWithSyncHandler()
        {
            var handler = ObjectFactory.GetInstance<RestHandler>();
            Assert.That(handler, Is.Not.Null);
            
            RequestContext requestContext = RequestContextMockFactory.Create(typeof(ITestService), SetUpFixture.RelativeUrl, "{id}", "DELETE");
            Assert.That(requestContext, Is.Not.Null);

            IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
            Assert.That(httpHandler, Is.Not.Null);
            Assert.That(httpHandler, Is.InstanceOf<RestHandler>());

            var request = ObjectFactory.GetInstance<IHttpRequest>();
            Assert.That(request, Is.Not.Null);
            request.SetRouteValues(new { id = 1 });

            ProcessRequest(handler);

            var response = ObjectFactory.GetInstance<IHttpResponse>();
            Assert.That(response, Is.Not.Null);
            Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void DeleteMethodWithAsyncHandler()
        {
            var handler = ObjectFactory.GetInstance<RestAsyncHandler>();
            Assert.That(handler, Is.Not.Null);
            
            RequestContext requestContext = RequestContextMockFactory.Create(typeof(ITestService), SetUpFixture.RelativeUrl, "{id}", "DELETE");
            Assert.That(requestContext, Is.Not.Null);

            IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
            Assert.That(httpHandler, Is.Not.Null);
            Assert.That(httpHandler, Is.InstanceOf<RestAsyncHandler>());

            var request = ObjectFactory.GetInstance<IHttpRequest>();
            Assert.That(request, Is.Not.Null);
            request.SetRouteValues(new { id = 1 });

            ProcessRequest(handler);

            var response = ObjectFactory.GetInstance<IHttpResponse>();
            Assert.That(response, Is.Not.Null);
            Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test, ExpectedException(typeof(HttpResponseException))]
        public void ClearMethodWithSyncHandler_ShouldFailDueToInvalidVerb()
        {
            var handler = ObjectFactory.GetInstance<RestHandler>();
            Assert.That(handler, Is.Not.Null);
            
            RequestContext requestContext = RequestContextMockFactory.Create(typeof(ITestService), SetUpFixture.RelativeUrl, "{id}", "CLEAR");
            Assert.That(requestContext, Is.Not.Null);

            IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
            Assert.That(httpHandler, Is.Not.Null);
            Assert.That(httpHandler, Is.InstanceOf<RestHandler>());

            var request = ObjectFactory.GetInstance<IHttpRequest>();
            Assert.That(request, Is.Not.Null);
            request.SetRouteValues(new { id = 1 });

            ProcessRequest(handler);

            var response = ObjectFactory.GetInstance<IHttpResponse>();
            Assert.That(response, Is.Not.Null);
            Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void OptionsMethodWithSyncHandler()
        {
            var handler = ObjectFactory.GetInstance<RestHandler>();
            Assert.That(handler, Is.Not.Null);

            RequestContext requestContext = RequestContextMockFactory.Create(typeof(ITestService), SetUpFixture.RelativeUrl, "{id}", "OPTIONS");
            Assert.That(requestContext, Is.Not.Null);

            IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
            Assert.That(httpHandler, Is.Not.Null);
            Assert.That(httpHandler, Is.InstanceOf<RestHandler>());

            var request = ObjectFactory.GetInstance<IHttpRequest>();
            Assert.That(request, Is.Not.Null);
            request.SetRouteValues(new { id = 1 });

            ProcessRequest(handler);

            var response = ObjectFactory.GetInstance<IHttpResponse>();
            Assert.That(response, Is.Not.Null);
            Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.GetHeader("Allow"), Is.EqualTo("DELETE, GET, HEAD, PATCH, PUT"));
        }

        [Test]
        public void OptionsMethodWithAsyncHandler()
        {
            var handler = ObjectFactory.GetInstance<RestAsyncHandler>();
            Assert.That(handler, Is.Not.Null);

            RequestContext requestContext = RequestContextMockFactory.Create(typeof(ITestService), SetUpFixture.RelativeUrl, "{id}", "OPTIONS");
            Assert.That(requestContext, Is.Not.Null);

            IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
            Assert.That(httpHandler, Is.Not.Null);
            Assert.That(httpHandler, Is.InstanceOf<RestAsyncHandler>());

            var request = ObjectFactory.GetInstance<IHttpRequest>();
            Assert.That(request, Is.Not.Null);
            request.SetRouteValues(new { id = 1 });

            ProcessRequest(handler);

            var response = ObjectFactory.GetInstance<IHttpResponse>();
            Assert.That(response, Is.Not.Null);
            Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.GetHeader("Allow"), Is.EqualTo("DELETE, GET, HEAD, PATCH, PUT"));
        }

        private static void ProcessRequest(IHttpHandler handler)
        {
            handler.ProcessRequest(null);
        }

        private static void ProcessRequest(IHttpAsyncHandler handler)
        {
            Exception asyncException = null;

            IAsyncResult result = handler.BeginProcessRequest(null, callback =>
                                                                    {
                                                                        try
                                                                        {
                                                                            handler.EndProcessRequest(callback);
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                            asyncException = ex;
                                                                        }
                                                                    }, null);

            result.AsyncWaitHandle.WaitOne();

            if (asyncException != null)
            {
                throw asyncException;
            }
        }
    }
}
