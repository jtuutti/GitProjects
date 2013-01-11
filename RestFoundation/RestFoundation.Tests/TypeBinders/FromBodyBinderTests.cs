using System;
using NUnit.Framework;
using RestFoundation.TypeBinders;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.TypeBinders
{
    [TestFixture]
    public class FromBodyBinderTests
    {
        private IServiceContext m_context;
        private ITypeBinder m_binder;

        [SetUp]
        public void Initialize()
        {
            m_context = MockContextManager.GenerateContext(HttpMethod.Post);
            m_binder = new FromBodyAttribute();
        }

        [TearDown]
        public void Destroy()
        {
            MockContextManager.DestroyContext();
        }

        [Test]
        public void Test_Object_Binding()
        {
            MockContextManager.SetFormData("name", "Dmitry");
            MockContextManager.SetFormData("age", "15");
            MockContextManager.SetFormData("id", Guid.NewGuid().ToString());

            var formData = m_context.GetHttpContext().Request.Form;
            Assert.That(formData["name"], Is.Not.Null);
            Assert.That(formData["name"], Is.Not.Empty);
            Assert.That(formData["age"], Is.Not.Null);
            Assert.That(formData["age"], Is.Not.Empty);
            Assert.That(formData["id"], Is.Not.Null);
            Assert.That(formData["id"], Is.Not.Empty);

            var name = m_binder.Bind("name", typeof(string), m_context) as string;
            Assert.That(name, Is.Not.Null);
            Assert.That(name, Is.EqualTo(formData["name"]));

            var age = (int) m_binder.Bind("age", typeof(int), m_context);
            Assert.That(age, Is.EqualTo(Int32.Parse(formData["age"])));

            var id = (Guid) m_binder.Bind("id", typeof(Guid), m_context);
            Assert.That(id, Is.EqualTo(Guid.Parse(formData["id"])));
        }

        [Test]
        public void Test_Array_Binding()
        {
            MockContextManager.SetFormData("id", "5");
            MockContextManager.SetFormData("id", "10");
            MockContextManager.SetFormData("id", "20");
            MockContextManager.SetFormData("id", "50");

            var formData = m_context.GetHttpContext().Request.Form;

            var idValues = formData.GetValues("id");
            Assert.That(idValues, Is.Not.Null);
            Assert.That(idValues.Length, Is.EqualTo(4));

            var stringIds = m_binder.Bind("id", typeof(string[]), m_context) as string[];
            Assert.That(stringIds, Is.Not.Null);
            Assert.That(stringIds.Length, Is.EqualTo(idValues.Length));

            foreach (var id in stringIds)
            {
                Assert.That(id, Is.Not.Null);
                Assert.That(id, Is.Not.Empty);
            }

            var decimalIds = m_binder.Bind("id", typeof(decimal[]), m_context) as decimal[];
            Assert.That(decimalIds, Is.Not.Null);
            Assert.That(decimalIds.Length, Is.EqualTo(idValues.Length));

            foreach (var id in decimalIds)
            {
                Assert.That(id, Is.GreaterThan(0));
            }
        }

        [Test]
        public void Test_Object_Binding_Over_Get()
        {
            MockContextManager.DestroyContext();

            var context = MockContextManager.GenerateContext(HttpMethod.Get);
            Assert.That(context, Is.Not.Null);
            Assert.Throws<HttpResponseException>(() => m_binder.Bind("name", typeof(string), context));
        }
    }
}
