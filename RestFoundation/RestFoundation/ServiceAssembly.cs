using System.Reflection;

namespace RestFoundation
{
    public static class ServiceAssembly
    {
        public static readonly Assembly Executing = typeof(ServiceAssembly).Assembly;
    }
}
