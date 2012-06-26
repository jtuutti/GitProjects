using System.Runtime.Serialization;
using NUnit.Framework;

namespace RestFoundation.Tests
{
    [TestFixture]
    public class ObjectActivatorTests
    {
        [Test]
        public void RegisteredActivator()
        {
            var activator = Rest.Configure.Activator;
            Assert.That(activator, Is.Not.Null);

            var registeredAbstraction = activator.Create(typeof(IServiceFactory)) as IServiceFactory;
            Assert.That(registeredAbstraction, Is.Not.Null);

            var unregisteredAbstraction = activator.Create(typeof(ISerializable)) as ISerializable;
            Assert.That(unregisteredAbstraction, Is.Null);

            var implementation = activator.Create(typeof(Operation)) as Operation;
            Assert.That(implementation, Is.Not.Null);
        }
    }
}
