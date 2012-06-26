using System;
using NUnit.Framework;
using RestFoundation.Test;
using RestFoundation.Tests.ServiceContracts;

namespace RestFoundation.Tests
{
    [TestFixture]
    public class RouteTests
    {
        [Test]
        public void TestValidRoutes()
        {
            RouteAssert.Url("~/test/1").WithHttpMethod(HttpMethod.Get).Invokes<ITestService>(s => s.Get(1));
            RouteAssert.Url("~/test/2").WithHttpMethod(HttpMethod.Head).Invokes<ITestService>(s => s.Get(2));
            RouteAssert.Url("~/test/all/name").WithHttpMethod(HttpMethod.Get).Invokes<ITestService>(s => s.GetAll("name"));
            RouteAssert.Url("~/test/all/age").WithHttpMethod(HttpMethod.Head).Invokes<ITestService>(s => s.GetAll("age"));
            RouteAssert.Url("~/test/new").WithHttpMethod(HttpMethod.Post).Invokes<ITestService>(s => s.Post());
            RouteAssert.Url("~/test/1").WithHttpMethod(HttpMethod.Put).Invokes<ITestService>(s => s.Put(1));
            RouteAssert.Url("~/test/2").WithHttpMethod(HttpMethod.Patch).Invokes<ITestService>(s => s.Patch(2));
            RouteAssert.Url("~/test/3").WithHttpMethod(HttpMethod.Delete).Invokes<ITestService>(s => s.Delete(3));
        }
        
        [Test]
        public void TestInvalidRoutes()
        {
            // invalid relative URL
            Assert.Throws(typeof(ArgumentException), () => RouteAssert.Url("/rest/test/1").WithHttpMethod(HttpMethod.Get).Invokes<ITestService>(s => s.Get(1)));

            // invalid service contract type
            Assert.Throws(typeof(RouteAssertException), () => RouteAssert.Url("~/").WithHttpMethod(HttpMethod.Get).Invokes<RouteTests>(s => s.TestInvalidRoutes()));

            // mismatched route
            Assert.Throws(typeof(RouteAssertException), () => RouteAssert.Url("~/").WithHttpMethod(HttpMethod.Get).Invokes<ITestService>(s => s.Get(null)));

            // mismatched http method
            Assert.Throws(typeof(RouteAssertException), () => RouteAssert.Url("~/test/1").WithHttpMethod(HttpMethod.Put).Invokes<ITestService>(s => s.Patch(1)));

            // mismatched parameter values
            Assert.Throws(typeof(RouteAssertException), () => RouteAssert.Url("~/test/1").WithHttpMethod(HttpMethod.Get).Invokes<ITestService>(s => s.Get(11)));
        }
    }
}
