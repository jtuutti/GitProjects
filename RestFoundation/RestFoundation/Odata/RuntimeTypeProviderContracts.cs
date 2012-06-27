// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace RestFoundation.Odata
{
    internal abstract class RuntimeTypeProviderContracts : IRuntimeTypeProvider
    {
        public Type Get(Type sourceType, IEnumerable<MemberInfo> properties)
        {
            if (sourceType == null) throw new ArgumentNullException("sourceType");
            if (properties == null) throw new ArgumentNullException("properties");

            throw new NotImplementedException();
        }
    }
}