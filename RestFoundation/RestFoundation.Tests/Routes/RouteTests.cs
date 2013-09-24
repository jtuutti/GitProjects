using System;
using NUnit.Framework;
using RestFoundation.Tests.Implementation.ServiceContracts;
using RestFoundation.Tests.Implementation.Services;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.Routes
{
    [TestFixture]
    public class RouteTests
    {
        [Test]
        public void ValidRoutes()
        {
            // default URL that supports GET and HEAD
            AssertThat.Url("~/test-service/1").WithHttpMethod(HttpMethod.Get).Invokes<ITestService>(s => s.Get(1));
            AssertThat.Url("~/test-service/2").WithHttpMethod(HttpMethod.Head).Invokes<ITestService>(s => s.Get(2));

            // default URL with additional query data
            AssertThat.Url("~/test-service/1?a=b&c=d").WithHttpMethod(HttpMethod.Get).Invokes<ITestService>(s => s.Get(1));
            AssertThat.Url("~/test-service/1#section1").WithHttpMethod(HttpMethod.Get).Invokes<ITestService>(s => s.Get(1));

            // "all" URL that supports GET and HEAD
            AssertThat.Url("~/test-service/all/name").WithHttpMethod(HttpMethod.Get).Invokes<ITestService>(s => s.GetAll("name"));
            AssertThat.Url("~/test-service/all/age").WithHttpMethod(HttpMethod.Head).Invokes<ITestService>(s => s.GetAll("age"));

            // POST URL
            AssertThat.Url("~/test-service/new").WithHttpMethod(HttpMethod.Post).Invokes<ITestService>(s => s.Post(null));

            // POST URL with additional query data
            AssertThat.Url("~/test-service/new?timestamp=1234567").WithHttpMethod(HttpMethod.Post).Invokes<ITestService>(s => s.Post(null));

            // PUT URL
            AssertThat.Url("~/test-service/1").WithHttpMethod(HttpMethod.Put).Invokes<ITestService>(s => s.Put(1));

            // PATCH URL
            AssertThat.Url("~/test-service/2").WithHttpMethod(HttpMethod.Patch).Invokes<ITestService>(s => s.Patch(2));

            // DELETE URL
            AssertThat.Url("~/test-service/3").WithHttpMethod(HttpMethod.Delete).Invokes<ITestService>(s => s.Delete(3));
        }

        [Test]
        public void InvalidRoutes()
        {
            // invalid relative URL
            Assert.Throws(typeof(ArgumentException), () => AssertThat.Url("/rest/test/1"));

            // invalid service contract type
            AssertThat.Url("~/").WithHttpMethod(HttpMethod.Get).FailsOnInvocation<RouteTests>(s => s.InvalidRoutes());

            // mismatched route
            AssertThat.Url("~/").WithHttpMethod(HttpMethod.Get).FailsOnInvocation<ITestService>(s => s.Get(null));

            // mismatched route due to using query data instead of the route parameter
            AssertThat.Url("~/test-service?id=1").WithHttpMethod(HttpMethod.Get).FailsOnInvocation<ITestService>(s => s.Get(1));

            // mismatched http method
            AssertThat.Url("~/test-service/1").WithHttpMethod(HttpMethod.Put).FailsOnInvocation<ITestService>(s => s.Patch(1));

            // mismatched parameter values
            AssertThat.Url("~/test-service/1").WithHttpMethod(HttpMethod.Get).FailsOnInvocation<ITestService>(s => s.Get(11));

            // invalid parameter type
            AssertThat.Url("~/test-service/a").WithHttpMethod(HttpMethod.Get).FailsOnInvocation<ITestService>(s => s.Get(1));

            // parameter constraint violation - orderby must start with a letter or underscore
            AssertThat.Url("~/test-service/all/1name").WithHttpMethod(HttpMethod.Get).FailsOnInvocation<ITestService>(s => s.GetAll("1name"));
        }

        [Test]
        public void SelfContainedServiceRoutes()
        {
            AssertThat.Url("~/test-ok-fail/ok").WithHttpMethod(HttpMethod.Get).Invokes<TestSelfContainedService>(s => s.GetOK());
            AssertThat.Url("~/test-ok-fail/fail").WithHttpMethod(HttpMethod.Get).Invokes<TestSelfContainedService>(s => s.GetFail());

            string expectedMessage = "URL '~/test-ok-fail/fail' does not match any routes.";
            AssertThat.Url("~/test-ok-fail/fail").WithHttpMethod(HttpMethod.Head).FailsOnInvocation<TestSelfContainedService>(s => s.GetFail(), expectedMessage);

            expectedMessage = "Invalid or mismatched service method lambda expression provided.";
            AssertThat.Url("~/test-ok-fail/ok").WithHttpMethod(HttpMethod.Get).FailsOnInvocation<TestSelfContainedService>(s => s.GetFail(), expectedMessage);
        }
    }
}
