// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System.Collections.Generic;

namespace RestFoundation.Odata
{
	internal interface IModelFilter<in T>
	{
		IEnumerable<object> Filter(IEnumerable<T> source);
	}
}