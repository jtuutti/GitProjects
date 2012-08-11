using NUnit.Framework;
using RestFoundation.Runtime;

namespace RestFoundation.Tests.Cache
{
    [TestFixture]
    public class CacheTests
    {
        private IServiceCache m_cache;

        [TestFixtureSetUp]
        public void Initialize()
        {
            m_cache = Rest.Active.ServiceLocator.GetService<IServiceCache>();
        }

        [TestFixtureTearDown]
        public void ShutDown()
        {
            m_cache.Clear();
        }

        [Test]
        public void CacheIsInitialized()
        {
            Assert.That(m_cache, Is.Not.Null);
        }

        [Test]
        public void StoreAndRetrieveData()
        {
            const string cacheKey = "test";
            const string httpMethod = "GET";
            const string description = "A test method";

            m_cache.Add(cacheKey, new Operation { RelativeUrlTemplate = cacheKey, HttpMethod = httpMethod });
            Assert.That(m_cache.Contains(cacheKey), Is.EqualTo(true));

            m_cache.Update(cacheKey, new Operation { RelativeUrlTemplate = cacheKey, HttpMethod = httpMethod, Description = description });
            Assert.That(m_cache.Contains(cacheKey), Is.EqualTo(true));

            var operation = (Operation) m_cache.Get(cacheKey);
            Assert.That(operation, Is.Not.Null);
            Assert.That(operation.RelativeUrlTemplate, Is.EqualTo(cacheKey));
            Assert.That(operation.HttpMethod, Is.EqualTo(httpMethod));
            Assert.That(operation.Description, Is.EqualTo(description));

            Assert.That(m_cache.Remove(cacheKey), Is.EqualTo(true));
        }
    }
}
