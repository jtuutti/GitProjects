using System.Reflection;

namespace RestFoundation
{
    public static class ServiceAssembly
    {
        public readonly static Assembly Executing = typeof(ServiceAssembly).Assembly;
    }
}
