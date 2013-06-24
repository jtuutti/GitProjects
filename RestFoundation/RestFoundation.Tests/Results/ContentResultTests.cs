using NUnit.Framework;
using RestFoundation.Results;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.Results
{
    [TestFixture]
    public class ContentResultTests : ResultTestBase
    {
        [SetUp]
        public void Initialize()
        {
            Context = MockContextManager.GenerateContext();
        }

        [TearDown]
        public void ShutDown()
        {
            MockContextManager.DestroyContext();
        }

        [Test]
        public void ContentResultWithPlainText()
        {
            const string ContentValue = "Hello world";
            const string ContentType = "text/plain";

            ContentResult result = Result.Content(ContentValue, true, ContentType);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ClearOutput, Is.True);
            Assert.That(result.Content, Is.EqualTo(ContentValue));
            Assert.That(result.ContentType, Is.EqualTo(ContentType));

            result.Execute(Context);
            Assert.That(GetResponseOutput(), Is.EqualTo(ContentValue));
        }

        [Test]
        public void ContentResultWithHtml()
        {
            const string ContentValue = "<div style='padding-top: 10px;'>Page executed</div>";
            const string ContentType = "text/html";

            ContentResult result = Result.Content(ContentValue, false, ContentType);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ClearOutput, Is.False);
            Assert.That(result.Content, Is.EqualTo(ContentValue));
            Assert.That(result.ContentType, Is.EqualTo(ContentType));

            result.Execute(Context);
            Assert.That(GetResponseOutput(), Is.EqualTo(ContentValue));
        }
    }
}
