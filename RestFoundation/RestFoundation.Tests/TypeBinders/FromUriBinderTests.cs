using System;
using NUnit.Framework;
using RestFoundation.TypeBinders;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.TypeBinders
{
    [TestFixture]
    public class FromUriBinderTests
    {
        private IServiceContext m_context;
        private ITypeBinder m_binder;

        [SetUp]
        public void Initialize()
        {
            m_context = MockContextManager.GenerateContext(HttpMethod.Post);
            m_binder = new FromUriAttribute();
        }

        [TearDown]
        public void Destroy()
        {
            MockContextManager.DestroyContext();
        }

        [Test]
        public void Test_Object_Binding()
        {
            MockContextManager.SetQuery("name", "Dmitry");
            MockContextManager.SetQuery("age", "15");
            MockContextManager.SetQuery("id", Guid.NewGuid().ToString());

            var queryString = m_context.Request.QueryString;
            Assert.That(queryString.TryGet("name"), Is.Not.Null);
            Assert.That(queryString.TryGet("name"), Is.Not.Empty);
            Assert.That(queryString.TryGet("age"), Is.Not.Null);
            Assert.That(queryString.TryGet("age"), Is.Not.Empty);
            Assert.That(queryString.TryGet("id"), Is.Not.Null);
            Assert.That(queryString.TryGet("id"), Is.Not.Empty);

            var name = m_binder.Bind("name", typeof(string), m_context) as string;
            Assert.That(name, Is.Not.Null);
            Assert.That(name, Is.EqualTo(queryString.Get("name")));

            var age = (int) m_binder.Bind("age", typeof(int), m_context);
            Assert.That(age, Is.EqualTo(Int32.Parse(queryString.Get("age"))));

            var id = (Guid) m_binder.Bind("id", typeof(Guid), m_context);
            Assert.That(id, Is.EqualTo(Guid.Parse(queryString.Get("id"))));
        }

        [Test]
        public void Test_Array_Binding()
        {
            MockContextManager.SetQuery("id", "5");
            MockContextManager.SetQuery("id", "10");
            MockContextManager.SetQuery("id", "20");
            MockContextManager.SetQuery("id", "50");

            var idValues = m_context.Request.QueryString.GetValues("id");
            Assert.That(idValues, Is.Not.Null);
            Assert.That(idValues.Count, Is.EqualTo(4));

            var stringIds = m_binder.Bind("id", typeof(string[]), m_context) as string[];
            Assert.That(stringIds, Is.Not.Null);
            Assert.That(stringIds.Length, Is.EqualTo(idValues.Count));

            foreach (var id in stringIds)
            {
                Assert.That(id, Is.Not.Null);
                Assert.That(id, Is.Not.Empty);
            }

            var decimalIds = m_binder.Bind("id", typeof(decimal[]), m_context) as decimal[];
            Assert.That(decimalIds, Is.Not.Null);
            Assert.That(decimalIds.Length, Is.EqualTo(idValues.Count));

            foreach (var id in decimalIds)
            {
                Assert.That(id, Is.GreaterThan(0));
            }
        }
    }
}
