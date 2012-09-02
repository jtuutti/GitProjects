﻿using System;
using NUnit.Framework;
using RestFoundation.Tests.Implementation.ServiceContracts;
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
            RouteAssert.Url("~/test-service/1").WithHttpMethod(HttpMethod.Get).Invokes<ITestService>(s => s.Get(1));
            RouteAssert.Url("~/test-service/2").WithHttpMethod(HttpMethod.Head).Invokes<ITestService>(s => s.Get(2));

            // default URL with additional query data
            RouteAssert.Url("~/test-service/1?a=b&c=d").WithHttpMethod(HttpMethod.Get).Invokes<ITestService>(s => s.Get(1));
            RouteAssert.Url("~/test-service/1#section1").WithHttpMethod(HttpMethod.Get).Invokes<ITestService>(s => s.Get(1));

            // "all" URL that supports GET and HEAD
            RouteAssert.Url("~/test-service/all/name").WithHttpMethod(HttpMethod.Get).Invokes<ITestService>(s => s.GetAll("name"));
            RouteAssert.Url("~/test-service/all/age").WithHttpMethod(HttpMethod.Head).Invokes<ITestService>(s => s.GetAll("age"));

            // POST URL
            RouteAssert.Url("~/test-service/new").WithHttpMethod(HttpMethod.Post).Invokes<ITestService>(s => s.Post(null));

            // POST URL with additional query data
            RouteAssert.Url("~/test-service/new?timestamp=1234567").WithHttpMethod(HttpMethod.Post).Invokes<ITestService>(s => s.Post(null));

            // PUT URL
            RouteAssert.Url("~/test-service/1").WithHttpMethod(HttpMethod.Put).Invokes<ITestService>(s => s.Put(1));

            // PATCH URL
            RouteAssert.Url("~/test-service/2").WithHttpMethod(HttpMethod.Patch).Invokes<ITestService>(s => s.Patch(2));

            // DELETE URL
            RouteAssert.Url("~/test-service/3").WithHttpMethod(HttpMethod.Delete).Invokes<ITestService>(s => s.Delete(3));
        }
        
        [Test]
        public void InvalidRoutes()
        {
            // invalid relative URL
            Assert.Throws(typeof(ArgumentException), () => RouteAssert.Url("/rest/test/1").WithHttpMethod(HttpMethod.Get).Invokes<ITestService>(s => s.Get(1)));

            // invalid service contract type
            Assert.Throws(typeof(RouteAssertException), () => RouteAssert.Url("~/").WithHttpMethod(HttpMethod.Get).Invokes<RouteTests>(s => s.InvalidRoutes()));

            // mismatched route
            Assert.Throws(typeof(RouteAssertException), () => RouteAssert.Url("~/").WithHttpMethod(HttpMethod.Get).Invokes<ITestService>(s => s.Get(null)));

            // mismatched route due to using query data instead of the route parameter
            Assert.Throws(typeof(RouteAssertException), () => RouteAssert.Url("~/test-service?id=1").WithHttpMethod(HttpMethod.Get).Invokes<ITestService>(s => s.Get(1)));

            // mismatched http method
            Assert.Throws(typeof(RouteAssertException), () => RouteAssert.Url("~/test-service/1").WithHttpMethod(HttpMethod.Put).Invokes<ITestService>(s => s.Patch(1)));

            // mismatched parameter values
            Assert.Throws(typeof(RouteAssertException), () => RouteAssert.Url("~/test-service/1").WithHttpMethod(HttpMethod.Get).Invokes<ITestService>(s => s.Get(11)));

            // invalid parameter type
            Assert.Throws(typeof(RouteAssertException), () => RouteAssert.Url("~/test-service/a").WithHttpMethod(HttpMethod.Get).Invokes<ITestService>(s => s.Get(1)));

            // parameter constraint violation - orderby must start with a letter or underscore
            Assert.Throws(typeof(RouteAssertException), () => RouteAssert.Url("~/test-service/all/1name").WithHttpMethod(HttpMethod.Get).Invokes<ITestService>(s => s.GetAll("1name")));
        }
    }
}