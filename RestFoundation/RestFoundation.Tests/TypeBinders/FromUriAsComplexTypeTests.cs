using System;
using NUnit.Framework;
using RestFoundation.Runtime;
using RestFoundation.Tests.Implementation.Models;
using RestFoundation.TypeBinders;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.TypeBinders
{
    [TestFixture]
    public class FromUriAsComplexTypeTests
    {
        private IServiceContext m_context;
        private ITypeBinder m_binder;

        [SetUp]
        public void Initialize()
        {
            m_context = MockContextManager.GenerateContext(HttpMethod.Post);
            m_binder = new FromUriAsComplexTypeAttribute();
        }

        [TearDown]
        public void Destroy()
        {
            MockContextManager.DestroyContext();
        }

        [Test]
        public void Test_Binding()
        {
            MockContextManager.SetQuery("name", "User");
            MockContextManager.SetQuery("id", "12");
            MockContextManager.SetQuery("items", "Age:15");
            MockContextManager.SetQuery("items", "Sex:M");
            MockContextManager.SetQuery("items", "Height:5'11\"");

            var queryString = m_context.Request.QueryString;
            Assert.That(queryString.TryGet("name"), Is.Not.Null);
            Assert.That(queryString.TryGet("name"), Is.Not.Empty);
            Assert.That(queryString.TryGet("id"), Is.Not.Null);
            Assert.That(queryString.TryGet("id"), Is.Not.Empty);
            Assert.That(queryString.GetValues("items").Count, Is.EqualTo(3));

            var model = m_binder.Bind("person", typeof(Model), m_context) as Model;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Name, Is.EqualTo(queryString.TryGet("name")));
            Assert.That(model.Id, Is.EqualTo(Int32.Parse(queryString.TryGet("id"))));
            Assert.That(model.Items.Length, Is.EqualTo(queryString.GetValues("items").Count));

            for (int i = 0; i < model.Items.Length; i++)
            {
                Assert.That(model.Items[i], Is.Not.Null);
                Assert.That(model.Items[i], Is.Not.Empty);
                Assert.That(model.Items[i], Is.EqualTo(queryString.GetValues("items")[i]));
            }
        }

        [Test]
        public void Test_Binding_Singularize()
        {
            MockContextManager.SetQuery("name", "User");
            MockContextManager.SetQuery("id", "12");
            MockContextManager.SetQuery("item", "Age:15");
            MockContextManager.SetQuery("item", "Sex:M");
            MockContextManager.SetQuery("item", "Height:5'11\"");

            var queryString = m_context.Request.QueryString;
            Assert.That(queryString.TryGet("name"), Is.Not.Null);
            Assert.That(queryString.TryGet("name"), Is.Not.Empty);
            Assert.That(queryString.TryGet("id"), Is.Not.Null);
            Assert.That(queryString.TryGet("id"), Is.Not.Empty);
            Assert.That(queryString.GetValues("item").Count, Is.EqualTo(3));

            var model = m_binder.Bind("person", typeof(Model), m_context) as Model;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Name, Is.EqualTo(queryString.TryGet("name")));
            Assert.That(model.Id, Is.EqualTo(Int32.Parse(queryString.TryGet("id"))));
            Assert.That(model.Items.Length, Is.EqualTo(queryString.GetValues("item").Count));

            for (int i = 0; i < model.Items.Length; i++)
            {
                Assert.That(model.Items[i], Is.Not.Null);
                Assert.That(model.Items[i], Is.Not.Empty);
                Assert.That(model.Items[i], Is.EqualTo(queryString.GetValues("item")[i]));
            }
        }

        [Test]
        public void Test_Binding_Invalid_Type_With_Assert()
        {
            MockContextManager.SetQuery("name", "User");
            MockContextManager.SetQuery("id", "1F");

            var queryString = m_context.Request.QueryString;
            Assert.That(queryString.TryGet("name"), Is.Not.Null);
            Assert.That(queryString.TryGet("name"), Is.Not.Empty);
            Assert.That(queryString.TryGet("id"), Is.Not.Null);
            Assert.That(queryString.TryGet("id"), Is.Not.Empty);

            ((FromUriAsComplexTypeAttribute) m_binder).AssertTypeConversion = true;

            Assert.Throws<HttpResourceFaultException>(() => m_binder.Bind("person", typeof(Model), m_context));
        }

        [Test]
        public void Test_Binding_Invalid_Type_Without_Assert()
        {
            MockContextManager.SetQuery("name", "User");
            MockContextManager.SetQuery("id", "1F");

            var queryString = m_context.Request.QueryString;
            Assert.That(queryString.TryGet("name"), Is.Not.Null);
            Assert.That(queryString.TryGet("name"), Is.Not.Empty);
            Assert.That(queryString.TryGet("id"), Is.Not.Null);
            Assert.That(queryString.TryGet("id"), Is.Not.Empty);

            var model = m_binder.Bind("person", typeof(Model), m_context) as Model;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Name, Is.EqualTo(queryString.TryGet("name")));
            Assert.That(model.Id, Is.Null);
        }
    }
}
