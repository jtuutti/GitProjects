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
        private FromHeaderAttribute m_binder;

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
            MockContextManager.SetHeader("X-Name", "User");
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
        public void Test_Object_Binding_WithName()
        {
            MockContextManager.SetHeader("XX-Name", "User");
            MockContextManager.SetHeader("XX-Age", "15");
            MockContextManager.SetHeader("XX-Id", Guid.NewGuid().ToString());

            var headers = m_context.Request.Headers;
            Assert.That(headers.TryGet("XX-Name"), Is.Not.Null);
            Assert.That(headers.TryGet("XX-Name"), Is.Not.Empty);
            Assert.That(headers.TryGet("XX-Age"), Is.Not.Null);
            Assert.That(headers.TryGet("XX-Age"), Is.Not.Empty);
            Assert.That(headers.TryGet("XX-Id"), Is.Not.Null);
            Assert.That(headers.TryGet("XX-Id"), Is.Not.Empty);

            m_binder.Name = "XX-Name";

            var name = m_binder.Bind("name", typeof(string), m_context) as string;
            Assert.That(name, Is.Not.Null);
            Assert.That(name, Is.EqualTo(headers.Get("XX-Name")));

            m_binder.Name = "XX-Age";

            var age = (int) m_binder.Bind("age", typeof(int), m_context);
            Assert.That(age, Is.EqualTo(Int32.Parse(headers.Get("XX-Age"))));

            m_binder.Name = "XX-Id";

            var id = (Guid) m_binder.Bind("id", typeof(Guid), m_context);
            Assert.That(id, Is.EqualTo(Guid.Parse(headers.Get("XX-Id"))));

            m_binder.Name = null;
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
