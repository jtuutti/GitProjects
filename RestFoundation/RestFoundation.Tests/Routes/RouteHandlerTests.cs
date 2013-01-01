using System;
using System.Net;
using NUnit.Framework;
using RestFoundation.Client;
using RestFoundation.Runtime.Handlers;
using RestFoundation.Tests.Implementation.Models;
using RestFoundation.Tests.Implementation.ServiceContracts;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.Routes
{
    [TestFixture]
    public class RouteHandlerTests
    {
        [Test]
        public void GetMethodWithSyncHandler()
        {
            const string url = "~/test-service/1";
            const HttpMethod method = HttpMethod.Get;

            using (var factory = new MockHandlerFactory())
            {
                IRestHandler handler = factory.Create<ITestService>(url, m => m.Get(1));
                Assert.That(handler, Is.Not.Null);
                Assert.That(handler, Is.InstanceOf<RestHandler>());
                Assert.That(handler.Context, Is.Not.Null);
                Assert.That(handler.Context.Request, Is.Not.Null);
                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Request.Url, Is.EqualTo(ToAbsoluteUri(url)));
                Assert.That(handler.Context.Request.Method, Is.EqualTo(method));

                ProcessRequest(handler);

                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
            }
        }

        [Test]
        public void GetMethodWithAsyncHandler()
        {
            const string url = "~/test-service/1";
            const HttpMethod method = HttpMethod.Get;

            using (var factory = new MockHandlerFactory())
            {
                IRestHandler handler = factory.CreateAsync<ITestService>(url, m => m.Get(1));
                Assert.That(handler, Is.Not.Null);
                Assert.That(handler, Is.InstanceOf<RestAsyncHandler>());
                Assert.That(handler.Context, Is.Not.Null);
                Assert.That(handler.Context.Request, Is.Not.Null);
                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Request.Url, Is.EqualTo(ToAbsoluteUri(url)));
                Assert.That(handler.Context.Request.Method, Is.EqualTo(method));

                ProcessRequest(handler);

                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
            }
        }

        [Test, ExpectedException(typeof(HttpResponseException))]
        public void GetMethodWithSyncHandler_ShouldFailDueToMismatchedRoute()
        {
            const string url = "~/test-service/a/b";
            const HttpMethod method = HttpMethod.Get;

            using (var factory = new MockHandlerFactory())
            {
                IRestHandler handler = factory.Create<ITestService>(url, m => m.Get(1));
                Assert.That(handler, Is.Not.Null);
                Assert.That(handler, Is.InstanceOf<RestHandler>());
                Assert.That(handler.Context, Is.Not.Null);
                Assert.That(handler.Context.Request, Is.Not.Null);
                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Request.Url, Is.EqualTo(ToAbsoluteUri(url)));
                Assert.That(handler.Context.Request.Method, Is.EqualTo(method));

                ProcessRequest(handler);

                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
            }
        }

        [Test, ExpectedException(typeof(HttpResponseException))]
        public void GetMethodWithSyncHandler_ShouldFailDueToNegativeIdValue()
        {
            const string url = "~/test-service/-1";
            const HttpMethod method = HttpMethod.Get;

            using (var factory = new MockHandlerFactory())
            {
                IRestHandler handler = factory.Create<ITestService>(url, m => m.Get(-1));
                Assert.That(handler, Is.Not.Null);
                Assert.That(handler, Is.InstanceOf<RestHandler>());
                Assert.That(handler.Context, Is.Not.Null);
                Assert.That(handler.Context.Request, Is.Not.Null);
                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Request.Url, Is.EqualTo(ToAbsoluteUri(url)));
                Assert.That(handler.Context.Request.Method, Is.EqualTo(method));

                ProcessRequest(handler);

                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
            }
        }

        [Test, ExpectedException(typeof(RouteAssertException))]
        public void GetMethodWithSyncHandler_ShouldFailDueToInvalidServiceMethod()
        {
            const string url = "~/test-service/1";
            const HttpMethod method = HttpMethod.Get;

            using (var factory = new MockHandlerFactory())
            {
                IRestHandler handler = factory.Create<ITestService>(url, m => m.GetAll("1"));
                Assert.That(handler, Is.Not.Null);
                Assert.That(handler, Is.InstanceOf<RestHandler>());
                Assert.That(handler.Context, Is.Not.Null);
                Assert.That(handler.Context.Request, Is.Not.Null);
                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Request.Url, Is.EqualTo(ToAbsoluteUri(url)));
                Assert.That(handler.Context.Request.Method, Is.EqualTo(method));

                ProcessRequest(handler);

                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
            }
        }

        [Test]
        public void HeadMethodWithSyncHandler()
        {
            const string url = "~/test-service/all/Name";
            const HttpMethod method = HttpMethod.Head;

            using (var factory = new MockHandlerFactory())
            {
                IRestHandler handler = factory.Create<ITestService>(url, m => m.GetAll("Name"), method);
                Assert.That(handler, Is.Not.Null);
                Assert.That(handler, Is.InstanceOf<RestHandler>());
                Assert.That(handler.Context, Is.Not.Null);
                Assert.That(handler.Context.Request, Is.Not.Null);
                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Request.Url, Is.EqualTo(ToAbsoluteUri(url)));
                Assert.That(handler.Context.Request.Method, Is.EqualTo(method));

                ProcessRequest(handler);

                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
            }
        }

        [Test]
        public void HeadMethodWithAsyncHandler()
        {
            const string url = "~/test-service/all/Name";
            const HttpMethod method = HttpMethod.Head;

            using (var factory = new MockHandlerFactory())
            {
                IRestHandler handler = factory.CreateAsync<ITestService>(url, m => m.GetAll("Name"), method);
                Assert.That(handler, Is.Not.Null);
                Assert.That(handler, Is.InstanceOf<RestAsyncHandler>());
                Assert.That(handler.Context, Is.Not.Null);
                Assert.That(handler.Context.Request, Is.Not.Null);
                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Request.Url, Is.EqualTo(ToAbsoluteUri(url)));
                Assert.That(handler.Context.Request.Method, Is.EqualTo(method));

                ProcessRequest(handler);

                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
            }
        }

        [Test]
        public void PostMethodWithSyncHandler()
        {
            const string url = "~/test-service/new";
            const HttpMethod method = HttpMethod.Post;

            using (var factory = new MockHandlerFactory())
            {
                IRestHandler handler = factory.Create<ITestService>(url, m => m.Post(null), method);
                Assert.That(handler, Is.Not.Null);
                Assert.That(handler, Is.InstanceOf<RestHandler>());
                Assert.That(handler.Context, Is.Not.Null);
                Assert.That(handler.Context.Request, Is.Not.Null);
                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Request.Url, Is.EqualTo(ToAbsoluteUri(url)));
                Assert.That(handler.Context.Request.Method, Is.EqualTo(method));

                factory.SetResource(CreateResource(), RestResourceType.Json);

                ProcessRequest(handler);

                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Response.GetStatusCode(), Is.EqualTo(HttpStatusCode.Created));
                Assert.That(handler.Context.Response.GetHeader("Location"), Is.EqualTo(String.Concat(handler.Context.Request.Url.OperationUrl, "/1")));
            }
        }

        [Test]
        public void PostMethodWithAsyncHandler()
        {
            const string url = "~/test-service/new";
            const HttpMethod method = HttpMethod.Post;

            using (var factory = new MockHandlerFactory())
            {
                IRestHandler handler = factory.CreateAsync<ITestService>(url, m => m.Post(null), method);
                Assert.That(handler, Is.Not.Null);
                Assert.That(handler, Is.InstanceOf<RestAsyncHandler>());
                Assert.That(handler.Context, Is.Not.Null);
                Assert.That(handler.Context.Request, Is.Not.Null);
                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Request.Url, Is.EqualTo(ToAbsoluteUri(url)));
                Assert.That(handler.Context.Request.Method, Is.EqualTo(method));

                factory.SetResource(CreateResource(), RestResourceType.Xml);

                ProcessRequest(handler);

                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Response.GetStatusCode(), Is.EqualTo(HttpStatusCode.Created));
                Assert.That(handler.Context.Response.GetHeader("Location"), Is.EqualTo(String.Concat(handler.Context.Request.Url.OperationUrl, "/1")));
            }
        }

        [Test, ExpectedException(typeof(HttpResponseException))]
        public void PostMethodWithSyncHandler_ShouldFailDueToMismatchedUrlTemplate()
        {
            const string url = "~/test-service/1";
            const HttpMethod method = HttpMethod.Post;

            using (var factory = new MockHandlerFactory())
            {
                IRestHandler handler = factory.Create<ITestService>(url, m => m.Post(null), method);
                Assert.That(handler, Is.Not.Null);
                Assert.That(handler, Is.InstanceOf<RestHandler>());
                Assert.That(handler.Context, Is.Not.Null);
                Assert.That(handler.Context.Request, Is.Not.Null);
                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Request.Url, Is.EqualTo(ToAbsoluteUri(url)));
                Assert.That(handler.Context.Request.Method, Is.EqualTo(method));

                ProcessRequest(handler);

                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Response.GetStatusCode(), Is.EqualTo(HttpStatusCode.Created));
                Assert.That(handler.Context.Response.GetHeader("Location"), Is.EqualTo(String.Concat(handler.Context.Request.Url.OperationUrl, "/1")));
            }
        }

        [Test]
        public void PutMethodWithSyncHandler()
        {
            const string url = "~/test-service/1";
            const HttpMethod method = HttpMethod.Put;

            using (var factory = new MockHandlerFactory())
            {
                IRestHandler handler = factory.Create<ITestService>(url, m => m.Put(1));
                Assert.That(handler, Is.Not.Null);
                Assert.That(handler, Is.InstanceOf<RestHandler>());
                Assert.That(handler.Context, Is.Not.Null);
                Assert.That(handler.Context.Request, Is.Not.Null);
                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Request.Url, Is.EqualTo(ToAbsoluteUri(url)));
                Assert.That(handler.Context.Request.Method, Is.EqualTo(method));

                ProcessRequest(handler);

                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Response.GetStatusCode(), Is.EqualTo(HttpStatusCode.NoContent));
            }
        }

        [Test]
        public void PutMethodWithAsyncHandler()
        {
            const string url = "~/test-service/1";
            const HttpMethod method = HttpMethod.Put;

            using (var factory = new MockHandlerFactory())
            {
                IRestHandler handler = factory.CreateAsync<ITestService>(url, m => m.Put(1));
                Assert.That(handler, Is.Not.Null);
                Assert.That(handler, Is.InstanceOf<RestAsyncHandler>());
                Assert.That(handler.Context, Is.Not.Null);
                Assert.That(handler.Context.Request, Is.Not.Null);
                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Request.Url, Is.EqualTo(ToAbsoluteUri(url)));
                Assert.That(handler.Context.Request.Method, Is.EqualTo(method));

                ProcessRequest(handler);

                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Response.GetStatusCode(), Is.EqualTo(HttpStatusCode.NoContent));
            }
        }

        [Test]
        public void PatchMethodWithSyncHandler()
        {
            const string url = "~/test-service/1";
            const HttpMethod method = HttpMethod.Patch;

            using (var factory = new MockHandlerFactory())
            {
                IRestHandler handler = factory.Create<ITestService>(url, m => m.Patch(1));
                Assert.That(handler, Is.Not.Null);
                Assert.That(handler, Is.InstanceOf<RestHandler>());
                Assert.That(handler.Context, Is.Not.Null);
                Assert.That(handler.Context.Request, Is.Not.Null);
                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Request.Url, Is.EqualTo(ToAbsoluteUri(url)));
                Assert.That(handler.Context.Request.Method, Is.EqualTo(method));

                ProcessRequest(handler);

                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Response.GetStatusCode(), Is.EqualTo(HttpStatusCode.NoContent));
            }
        }

        [Test]
        public void PatchMethodWithAsyncHandler()
        {
            const string url = "~/test-service/1";
            const HttpMethod method = HttpMethod.Patch;

            using (var factory = new MockHandlerFactory())
            {
                IRestHandler handler = factory.CreateAsync<ITestService>(url, m => m.Patch(1));
                Assert.That(handler, Is.Not.Null);
                Assert.That(handler, Is.InstanceOf<RestAsyncHandler>());
                Assert.That(handler.Context, Is.Not.Null);
                Assert.That(handler.Context.Request, Is.Not.Null);
                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Request.Url, Is.EqualTo(ToAbsoluteUri(url)));
                Assert.That(handler.Context.Request.Method, Is.EqualTo(method));

                ProcessRequest(handler);

                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Response.GetStatusCode(), Is.EqualTo(HttpStatusCode.NoContent));
            }
        }

        [Test]
        public void DeleteMethodWithSyncHandler()
        {
            const string url = "~/test-service/1";
            const HttpMethod method = HttpMethod.Delete;

            using (var factory = new MockHandlerFactory())
            {
                IRestHandler handler = factory.Create<ITestService>(url, m => m.Delete(1));
                Assert.That(handler, Is.Not.Null);
                Assert.That(handler, Is.InstanceOf<RestHandler>());
                Assert.That(handler.Context, Is.Not.Null);
                Assert.That(handler.Context.Request, Is.Not.Null);
                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Request.Url, Is.EqualTo(ToAbsoluteUri(url)));
                Assert.That(handler.Context.Request.Method, Is.EqualTo(method));

                ProcessRequest(handler);

                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Response.GetStatusCode(), Is.EqualTo(HttpStatusCode.NoContent));
            }
        }

        [Test]
        public void DeleteMethodWithAsyncHandler()
        {
            const string url = "~/test-service/1";
            const HttpMethod method = HttpMethod.Delete;

            using (var factory = new MockHandlerFactory())
            {
                IRestHandler handler = factory.CreateAsync<ITestService>(url, m => m.Delete(1));
                Assert.That(handler, Is.Not.Null);
                Assert.That(handler, Is.InstanceOf<RestAsyncHandler>());
                Assert.That(handler.Context, Is.Not.Null);
                Assert.That(handler.Context.Request, Is.Not.Null);
                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Request.Url, Is.EqualTo(ToAbsoluteUri(url)));
                Assert.That(handler.Context.Request.Method, Is.EqualTo(method));

                ProcessRequest(handler);

                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Response.GetStatusCode(), Is.EqualTo(HttpStatusCode.NoContent));
            }
        }

        [Test]
        public void OptionsMethodWithSyncHandler()
        {
            const string url = "~/test-service/1";
            const HttpMethod method = HttpMethod.Options;

            using (var factory = new MockHandlerFactory())
            {
                IRestHandler handler = factory.Create<ITestService>(url, m => m.Get(1), method);
                Assert.That(handler, Is.Not.Null);
                Assert.That(handler, Is.InstanceOf<RestHandler>());
                Assert.That(handler.Context, Is.Not.Null);
                Assert.That(handler.Context.Request, Is.Not.Null);
                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Request.Url, Is.EqualTo(ToAbsoluteUri(url)));
                Assert.That(handler.Context.Request.Method, Is.EqualTo(method));

                ProcessRequest(handler);

                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
                Assert.That(handler.Context.Response.GetHeader("Allow"), Is.EqualTo("DELETE, GET, HEAD, PATCH, PUT"));
            }
        }

        [Test]
        public void OptionsMethodWithAsyncHandler()
        {
            const string url = "~/test-service/new";
            const HttpMethod method = HttpMethod.Options;

            using (var factory = new MockHandlerFactory())
            {
                IRestHandler handler = factory.CreateAsync<ITestService>(url, m => m.Post(null), method);
                Assert.That(handler, Is.Not.Null);
                Assert.That(handler, Is.InstanceOf<RestAsyncHandler>());
                Assert.That(handler.Context, Is.Not.Null);
                Assert.That(handler.Context.Request, Is.Not.Null);
                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Request.Url, Is.EqualTo(ToAbsoluteUri(url)));
                Assert.That(handler.Context.Request.Method, Is.EqualTo(method));

                ProcessRequest(handler);

                Assert.That(handler.Context.Response, Is.Not.Null);
                Assert.That(handler.Context.Response.GetStatusCode(), Is.EqualTo(HttpStatusCode.OK));
                Assert.That(handler.Context.Response.GetHeader("Allow"), Is.EqualTo("POST"));
            }
        }

        private static void ProcessRequest(IRestHandler handler)
        {
            var asyncHandler = handler as IRestAsyncHandler;

            if (asyncHandler == null)
            {
                handler.ProcessRequest(null);
                return;
            }

            Exception asyncException = null;

            IAsyncResult result = asyncHandler.BeginProcessRequest(null, callback =>
                                                                         {
                                                                             try
                                                                             {
                                                                                 asyncHandler.EndProcessRequest(callback);
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

        private static Uri ToAbsoluteUri(string url)
        {
            return new Uri(new Uri("http://localhost"), url.TrimStart('~'));
        }

        private static Model CreateResource()
        {
            return new Model
            {
                Id = 1,
                Name = "Joe Bloe"
            };
        }
    }
}
