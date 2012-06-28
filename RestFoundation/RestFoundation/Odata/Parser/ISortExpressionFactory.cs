// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;

namespace RestFoundation.Odata.Parser
{
	internal interface ISortExpressionFactory
	{
		IEnumerable<SortDescription<T>> Create<T>(string filter);
		IEnumerable<SortDescription<T>> Create<T>(string filter, IFormatProvider formatProvider);
	}
}