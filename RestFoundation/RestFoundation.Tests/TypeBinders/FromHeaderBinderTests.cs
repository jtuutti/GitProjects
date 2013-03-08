using System;
using NUnit.Framework;
using RestFoundation.Runtime;
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
            MockContextManager.SetHeader("X-Name", "Dmitry");
            MockContextManager.SetHeader("X-Age", "15");
            MockContextManager.SetHeader("X-Id", Guid.NewGuid().ToString());

            var headers = m_context.Request.Headers;
            Assert.That(headers.TryGet("X-Name"), Is.Not.Null);
            Assert.That(headers.TryGet("X-Name"), Is.Not.Empty);
            Assert.That(headers.TryGet("X-Age"), Is.Not.Null);
            Assert.That(headers.TryGet("X-Age"), Is.Not.Empty);
            Assert.That(headers.TryGet("X-Id"), Is.Not.Null);
            Assert.That(headers.TryGet("X-Id"), Is.Not.Empty);

            var name = m_binder.Bind("X-Name", typeof(string), m_context) as string;
            Assert.That(name, Is.Not.Null);
            Assert.That(name, Is.EqualTo(headers.Get("X-Name")));

            var age = (int) m_binder.Bind("X-Age", typeof(int), m_context);
            Assert.That(age, Is.EqualTo(Int32.Parse(headers.Get("X-Age"))));

            var id = (Guid) m_binder.Bind("X-Id", typeof(Guid), m_context);
            Assert.That(id, Is.EqualTo(Guid.Parse(headers.Get("X-Id"))));
        }

        [Test]
        public void Test_Array_Binding()
        {
            MockContextManager.SetHeader("X-Id", "5");
            MockContextManager.SetHeader("X-Id", "10");
            MockContextManager.SetHeader("X-Id", "20");
            MockContextManager.SetHeader("X-Id", "50");

            var idValues = m_context.Request.Headers.GetValues("X-Id");
            Assert.That(idValues, Is.Not.Null);
            Assert.That(idValues.Count, Is.EqualTo(4));

            // Array binding is not supported for header data
            Assert.Throws<HttpResponseException>(() => m_binder.Bind("X-Id", typeof(string[]), m_context));
        }
    }
}
