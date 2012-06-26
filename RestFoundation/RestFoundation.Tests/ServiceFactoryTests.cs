using NUnit.Framework;
using RestFoundation.Tests.ServiceContracts;
using RestFoundation.Tests.Services;
using StructureMap;

namespace RestFoundation.Tests
{
    [TestFixture]
    public class ServiceFactoryTests
    {
        private IServiceFactory m_serviceFactory;

        [TestFixtureSetUp]
        public void Initialize()
        {
            m_serviceFactory = ObjectFactory.GetInstance<IServiceFactory>();
        }

        [Test]
        public void FactoryIsInitialized()
        {
            Assert.That(m_serviceFactory, Is.Not.Null);
        }

        [Test]
        public void InstantiateServiceFromContract()
        {
            var service = m_serviceFactory.Create(typeof(ITestService)) as ITestService;
            Assert.That(service, Is.Not.Null);

            var serviceImpl = (TestService) service;
            Assert.That(serviceImpl, Is.Not.Null);
            Assert.That(serviceImpl.Context, Is.Not.Null);
            Assert.That(serviceImpl.Request, Is.Not.Null);
            Assert.That(serviceImpl.Response, Is.Not.Null);
            Assert.That(serviceImpl.Response.Output, Is.Not.Null);
        }
    }
}
