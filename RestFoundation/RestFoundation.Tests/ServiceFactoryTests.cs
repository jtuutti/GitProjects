using NUnit.Framework;
using RestFoundation.Tests.ServiceContracts;
using RestFoundation.Tests.Services;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests
{
    [TestFixture]
    public class ServiceFactoryTests
    {
        private IServiceFactory m_serviceFactory;

        [TestFixtureSetUp]
        public void Initialize()
        {
            m_serviceFactory = Rest.Configure.GetImplementation<IServiceFactory>();
        }

        [Test]
        public void FactoryIsInitialized()
        {
            Assert.That(m_serviceFactory, Is.Not.Null);
        }

        [Test]
        public void InstantiateServiceFromContract()
        {
            var context = Rest.Configure.GetImplementation<IServiceContext>();
            Assert.That(context, Is.Not.Null);

            var service = m_serviceFactory.Create(typeof(ITestService), context) as ITestService;
            Assert.That(service, Is.Not.Null);

            var serviceImpl = (TestService) service;
            Assert.That(serviceImpl, Is.Not.Null);
            Assert.That(serviceImpl.Context, Is.Not.Null);
            Assert.That(serviceImpl.Context.Request, Is.Not.Null);
            Assert.That(serviceImpl.Context.Response, Is.Not.Null);
            Assert.That(serviceImpl.Context.Response.Output, Is.Not.Null);
        }
    }
}
