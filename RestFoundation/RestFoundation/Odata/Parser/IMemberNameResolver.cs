// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System;
using System.Reflection;

namespace RestFoundation.Odata.Parser
{
    internal interface IMemberNameResolver
    {
        string ResolveName(MemberInfo member);
    }

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