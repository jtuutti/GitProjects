using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace RestFoundation.Odata.Parser
{
    [ExcludeFromCodeCoverage]
    internal abstract class MemberNameResolverContracts : IMemberNameResolver
    {
        public string ResolveName(MemberInfo member)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }

            throw new NotImplementedException();
        }
    }
}