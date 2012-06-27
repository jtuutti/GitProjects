// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;

namespace RestFoundation.Odata
{
    internal abstract class ModelFilterContracts<T> : IModelFilter<T>
    {
        public IEnumerable<object> Filter(IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            throw new NotImplementedException();
        }
    }
}
