using System;
using NUnit.Framework;
using RestFoundation.TypeBinders;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.TypeBinders
{
    [TestFixture]
    public class FromHeaderBinderTests
    {
        private IServiceContext m_context;
        private ITypeBinder m_binder;

        [SetUp]
        public void Initialize()
        {
            m_context = MockContextManager.GenerateContext(HttpMethod.Post);
            m_binder = new FromHeaderAttribute();
        }

        [TearDown]
        public void Destroy()
        {
            MockContextManager.DestroyContext();
        }

        [Test]
        public void Test_Object_Binding()
        {
            var httpContext = m_context.GetHttpContext().Request;
            Assert.That(httpContext, Is.Not.Null);

            var headerData = httpContext.Headers;
            headerData.Add("X-Name", "Dmitry");
            headerData.Add("X-Age", "15");
            headerData.Add("X-Id", Guid.NewGuid().ToString());

            Assert.That(m_context.Request.Headers.TryGet("X-Name"), Is.Not.Null);
            Assert.That(m_context.Request.Headers.TryGet("X-Name"), Is.Not.Empty);
            Assert.That(m_context.Request.Headers.TryGet("X-Age"), Is.Not.Null);
            Assert.That(m_context.Request.Headers.TryGet("X-Age"), Is.Not.Empty);
            Assert.That(m_context.Request.Headers.TryGet("X-Id"), Is.Not.Null);
            Assert.That(m_context.Request.Headers.TryGet("X-Id"), Is.Not.Empty);

            var name = m_binder.Bind("X-Name", typeof(string), m_context) as string;
            Assert.That(name, Is.Not.Null);
            Assert.That(name, Is.EqualTo(headerData["X-Name"]));

            var age = (int) m_binder.Bind("X-Age", typeof(int), m_context);
            Assert.That(age, Is.EqualTo(Int32.Parse(headerData["X-Age"])));

            var id = (Guid) m_binder.Bind("X-Id", typeof(Guid), m_context);
            Assert.That(id, Is.EqualTo(Guid.Parse(headerData["X-Id"])));
        }

        [Test]
        public void Test_Array_Binding()
        {
            var httpContext = m_context.GetHttpContext().Request;
            Assert.That(httpContext, Is.Not.Null);

            var queryData = httpContext.QueryString;
            queryData.Add("X-Id", "5");
            queryData.Add("X-Id", "10");
            queryData.Add("X-Id", "50");
            queryData.Add("X-Id", "20");

            var idValues = m_context.Request.QueryString.GetValues("X-Id");
            Assert.That(idValues, Is.Not.Null);
            Assert.That(idValues.Count, Is.EqualTo(4));

            // Array binding is not supported for header data
            Assert.Throws<HttpResponseException>(() => m_binder.Bind("X-Id", typeof(string[]), m_context));
        }
    }
}
