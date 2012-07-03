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
            using (var factory = new MockContextFactory())
            {
                var handler = ObjectFactory.GetInstance<RestHandler>();
                Assert.That(handler, Is.Not.Null);

                var requestContext = factory.Create<ITestService>("~/test-service/1", m => m.Get(1));
                Assert.That(requestContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Response, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request.HttpMethod, Is.EqualTo("GET"));

                IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
                Assert.That(httpHandler, Is.Not.Null);
                Assert.That(httpHandler, Is.InstanceOf<RestHandler>());

                ProcessRequest(handler);

                var response = ObjectFactory.GetInstance<IHttpResponse>();
                Assert.That(response, Is.Not.Null);
                Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
            }
        }

        [Test]
        public void GetMethodWithAsyncHandler()
        {
            using (var factory = new MockContextFactory())
            {
                var handler = ObjectFactory.GetInstance<RestAsyncHandler>();
                Assert.That(handler, Is.Not.Null);

                RequestContext requestContext = factory.Create<ITestService>("~/test-service/1", m => m.Get(1));
                Assert.That(requestContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Response, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request.HttpMethod, Is.EqualTo("GET"));

                IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
                Assert.That(httpHandler, Is.Not.Null);
                Assert.That(httpHandler, Is.InstanceOf<RestAsyncHandler>());

                ProcessRequest(handler);

                var response = ObjectFactory.GetInstance<IHttpResponse>();
                Assert.That(response, Is.Not.Null);
                Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
            }
        }

        [Test, ExpectedException(typeof(HttpResponseException))]
        public void GetMethodWithSyncHandler_ShouldFailDueToMismatchedRoute()
        {
            using (var factory = new MockContextFactory())
            {
                var handler = ObjectFactory.GetInstance<RestHandler>();
                Assert.That(handler, Is.Not.Null);

                RequestContext requestContext = factory.Create<ITestService>("~/test-service/a/b", m => m.Get(-1));
                Assert.That(requestContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Response, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request.HttpMethod, Is.EqualTo("GET"));

                IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
                Assert.That(httpHandler, Is.Not.Null);
                Assert.That(httpHandler, Is.InstanceOf<RestHandler>());

                ProcessRequest(handler);

                var response = ObjectFactory.GetInstance<IHttpResponse>();
                Assert.That(response, Is.Not.Null);
                Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
            }
        }

        [Test, ExpectedException(typeof(HttpResponseException))]
        public void GetMethodWithSyncHandler_ShouldFailDueToNegativeIdValue()
        {
            using (var factory = new MockContextFactory())
            {
                var handler = ObjectFactory.GetInstance<RestHandler>();
                Assert.That(handler, Is.Not.Null);

                RequestContext requestContext = factory.Create<ITestService>("~/test-service/-1", m => m.Get(-1));
                Assert.That(requestContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Response, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request.HttpMethod, Is.EqualTo("GET"));

                IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
                Assert.That(httpHandler, Is.Not.Null);
                Assert.That(httpHandler, Is.InstanceOf<RestHandler>());

                ProcessRequest(handler);

                var response = ObjectFactory.GetInstance<IHttpResponse>();
                Assert.That(response, Is.Not.Null);
                Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
            }
        }

        [Test, ExpectedException(typeof(NotSupportedException))]
        public void GetMethodWithAsyncHandler_ShouldFailDueToCallingSynchronousProcessRequest()
        {
            using (var factory = new MockContextFactory())
            {
                var handler = ObjectFactory.GetInstance<RestAsyncHandler>();
                Assert.That(handler, Is.Not.Null);

                RequestContext requestContext = factory.Create<ITestService>("~/test-service/1", m => m.Get(-1));
                Assert.That(requestContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Response, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request.HttpMethod, Is.EqualTo("GET"));

                IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
                Assert.That(httpHandler, Is.Not.Null);
                Assert.That(httpHandler, Is.InstanceOf<RestAsyncHandler>());

                handler.ProcessRequest(null);

                var response = ObjectFactory.GetInstance<IHttpResponse>();
                Assert.That(response, Is.Not.Null);
                Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
            }
        }

        [Test]
        public void HeadMethodWithSyncHandler()
        {
            using (var factory = new MockContextFactory())
            {
                var handler = ObjectFactory.GetInstance<RestHandler>();
                Assert.That(handler, Is.Not.Null);

                RequestContext requestContext = factory.Create<ITestService>("~/test-service/all/Name", m => m.GetAll("Name"), HttpMethod.Head);
                Assert.That(requestContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Response, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request.HttpMethod, Is.EqualTo("HEAD"));

                requestContext.RouteData.Values["orderBy"] = "Name";

                IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
                Assert.That(httpHandler, Is.Not.Null);
                Assert.That(httpHandler, Is.InstanceOf<RestHandler>());

                ProcessRequest(handler);

                var response = ObjectFactory.GetInstance<IHttpResponse>();
                Assert.That(response, Is.Not.Null);
                Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
            }
        }

        [Test]
        public void HeadMethodWithAsyncHandler()
        {
            using (var factory = new MockContextFactory())
            {
                var handler = ObjectFactory.GetInstance<RestAsyncHandler>();
                Assert.That(handler, Is.Not.Null);

                RequestContext requestContext = factory.Create<ITestService>("~/test-service/all/Name", m => m.GetAll("Name"), HttpMethod.Head);
                Assert.That(requestContext, Is.Not.Null);
                Assert.That(requestContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Response, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request.HttpMethod, Is.EqualTo("HEAD"));

                requestContext.RouteData.Values["orderBy"] = "Name";

                IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
                Assert.That(httpHandler, Is.Not.Null);
                Assert.That(httpHandler, Is.InstanceOf<RestAsyncHandler>());

                ProcessRequest(handler);

                var response = ObjectFactory.GetInstance<IHttpResponse>();
                Assert.That(response, Is.Not.Null);
                Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
            }
        }

        [Test]
        public void PostMethodWithSyncHandler()
        {
            using (var factory = new MockContextFactory())
            {
                var handler = ObjectFactory.GetInstance<RestHandler>();
                Assert.That(handler, Is.Not.Null);

                RequestContext requestContext = factory.Create<ITestService>("~/test-service/new", m => m.Post());
                Assert.That(requestContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Response, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request.HttpMethod, Is.EqualTo("POST"));

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
        }

        [Test, ExpectedException(typeof(HttpResponseException))]
        public void PostMethodWithSyncHandler_ShouldFailDueToMismatchedUrlTemplate()
        {
            using (var factory = new MockContextFactory())
            {
                var handler = ObjectFactory.GetInstance<RestHandler>();
                Assert.That(handler, Is.Not.Null);

                RequestContext requestContext = factory.Create<ITestService>("~/test-service/1", m => m.Post());
                Assert.That(requestContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Response, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request.HttpMethod, Is.EqualTo("POST"));

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
        }

        [Test]
        public void PostMethodWithAsyncHandler()
        {
            using (var factory = new MockContextFactory())
            {
                var handler = ObjectFactory.GetInstance<RestAsyncHandler>();
                Assert.That(handler, Is.Not.Null);

                RequestContext requestContext = factory.Create<ITestService>("~/test-service/new", m => m.Post());
                Assert.That(requestContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Response, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request.HttpMethod, Is.EqualTo("POST"));

                var request = ObjectFactory.GetInstance<IHttpRequest>();
                Assert.That(request, Is.Not.Null);

                IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
                Assert.That(httpHandler, Is.Not.Null);
                Assert.That(httpHandler, Is.InstanceOf<RestAsyncHandler>());

                ProcessRequest(handler);

                var response = ObjectFactory.GetInstance<IHttpResponse>();
                Assert.That(response, Is.Not.Null);
                Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.Created));
                Assert.That(response.GetHeader("Location"), Is.EqualTo(String.Concat(request.Url.ServiceUrl, "/1")));
            }
        }

        [Test]
        public void PutMethodWithSyncHandler()
        {
            using (var factory = new MockContextFactory())
            {
                var handler = ObjectFactory.GetInstance<RestHandler>();
                Assert.That(handler, Is.Not.Null);

                RequestContext requestContext = factory.Create<ITestService>("~/test-service/1", m => m.Put(1));
                Assert.That(requestContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Response, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request.HttpMethod, Is.EqualTo("PUT"));

                IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
                Assert.That(httpHandler, Is.Not.Null);
                Assert.That(httpHandler, Is.InstanceOf<RestHandler>());

                ProcessRequest(handler);

                var response = ObjectFactory.GetInstance<IHttpResponse>();
                Assert.That(response, Is.Not.Null);
                Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.NoContent));
            }
        }

        [Test]
        public void PutMethodWithAsyncHandler()
        {
            using (var factory = new MockContextFactory())
            {
                var handler = ObjectFactory.GetInstance<RestAsyncHandler>();
                Assert.That(handler, Is.Not.Null);

                RequestContext requestContext = factory.Create<ITestService>("~/test-service/1", m => m.Put(1));
                Assert.That(requestContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Response, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request.HttpMethod, Is.EqualTo("PUT"));

                IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
                Assert.That(httpHandler, Is.Not.Null);
                Assert.That(httpHandler, Is.InstanceOf<RestAsyncHandler>());

                ProcessRequest(handler);

                var response = ObjectFactory.GetInstance<IHttpResponse>();
                Assert.That(response, Is.Not.Null);
                Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.NoContent));
            }
        }

        [Test]
        public void PatchMethodWithSyncHandler()
        {
            using (var factory = new MockContextFactory())
            {
                var handler = ObjectFactory.GetInstance<RestHandler>();
                Assert.That(handler, Is.Not.Null);

                RequestContext requestContext = factory.Create<ITestService>("~/test-service/1", m => m.Patch(1));
                Assert.That(requestContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Response, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request.HttpMethod, Is.EqualTo("PATCH"));

                IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
                Assert.That(httpHandler, Is.Not.Null);
                Assert.That(httpHandler, Is.InstanceOf<RestHandler>());

                ProcessRequest(handler);

                var response = ObjectFactory.GetInstance<IHttpResponse>();
                Assert.That(response, Is.Not.Null);
                Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.NoContent));
            }
        }

        [Test]
        public void PatchMethodWithAsyncHandler()
        {
            using (var factory = new MockContextFactory())
            {
                var handler = ObjectFactory.GetInstance<RestAsyncHandler>();
                Assert.That(handler, Is.Not.Null);

                RequestContext requestContext = factory.Create<ITestService>("~/test-service/1", m => m.Patch(1));
                Assert.That(requestContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Response, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request.HttpMethod, Is.EqualTo("PATCH"));

                IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
                Assert.That(httpHandler, Is.Not.Null);
                Assert.That(httpHandler, Is.InstanceOf<RestAsyncHandler>());

                ProcessRequest(handler);

                var response = ObjectFactory.GetInstance<IHttpResponse>();
                Assert.That(response, Is.Not.Null);
                Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.NoContent));
            }
        }

        [Test]
        public void DeleteMethodWithSyncHandler()
        {
            using (var factory = new MockContextFactory())
            {
                var handler = ObjectFactory.GetInstance<RestHandler>();
                Assert.That(handler, Is.Not.Null);

                RequestContext requestContext = factory.Create<ITestService>("~/test-service/1", m => m.Delete(1));
                Assert.That(requestContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Response, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request.HttpMethod, Is.EqualTo("DELETE"));

                IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
                Assert.That(httpHandler, Is.Not.Null);
                Assert.That(httpHandler, Is.InstanceOf<RestHandler>());

                ProcessRequest(handler);

                var response = ObjectFactory.GetInstance<IHttpResponse>();
                Assert.That(response, Is.Not.Null);
                Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.NoContent));
            }
        }

        [Test]
        public void DeleteMethodWithAsyncHandler()
        {
            using (var factory = new MockContextFactory())
            {
                var handler = ObjectFactory.GetInstance<RestAsyncHandler>();
                Assert.That(handler, Is.Not.Null);

                RequestContext requestContext = factory.Create<ITestService>("~/test-service/1", m => m.Delete(1));
                Assert.That(requestContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Response, Is.Not.Null);
                Assert.That(requestContext.HttpContext.Request.HttpMethod, Is.EqualTo("DELETE"));

                IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
                Assert.That(httpHandler, Is.Not.Null);
                Assert.That(httpHandler, Is.InstanceOf<RestAsyncHandler>());

                ProcessRequest(handler);

                var response = ObjectFactory.GetInstance<IHttpResponse>();
                Assert.That(response, Is.Not.Null);
                Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.NoContent));
            }
        }

        [Test]
        public void OptionsMethodWithSyncHandler()
        {
            using (var factory = new MockContextFactory())
            {
                var handler = ObjectFactory.GetInstance<RestHandler>();
                Assert.That(handler, Is.Not.Null);

                RequestContext requestContext = factory.Create<ITestService>("~/test-service/1", m => m.Get(1), HttpMethod.Options);
                Assert.That(requestContext, Is.Not.Null);

                IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
                Assert.That(httpHandler, Is.Not.Null);
                Assert.That(httpHandler, Is.InstanceOf<RestHandler>());

                ProcessRequest(handler);

                var response = ObjectFactory.GetInstance<IHttpResponse>();
                Assert.That(response, Is.Not.Null);
                Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.GetHeader("Allow"), Is.EqualTo("DELETE, GET, HEAD, PATCH, PUT"));
            }
        }

        [Test]
        public void OptionsMethodWithAsyncHandler()
        {
            using (var factory = new MockContextFactory())
            {
                var handler = ObjectFactory.GetInstance<RestAsyncHandler>();
                Assert.That(handler, Is.Not.Null);

                RequestContext requestContext = factory.Create<ITestService>("~/test-service/new", m => m.Post(), HttpMethod.Options);
                Assert.That(requestContext, Is.Not.Null);

                IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);
                Assert.That(httpHandler, Is.Not.Null);
                Assert.That(httpHandler, Is.InstanceOf<RestAsyncHandler>());

                ProcessRequest(handler);

                var response = ObjectFactory.GetInstance<IHttpResponse>();
                Assert.That(response, Is.Not.Null);
                Assert.That(response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.GetHeader("Allow"), Is.EqualTo("POST"));
            }
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
