using StructureMap;

namespace MessageBus.Mvc
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            ObjectFactory.Initialize(x => x.Scan(scan =>
                                                 {
                                                     scan.TheCallingAssembly();
                                                     scan.WithDefaultConventions();
                                                 }));

            BusConfiguration.Configure.UseStructureMap(typeof(IoC).Assembly);

            return ObjectFactory.Container;
        }
    }
}
