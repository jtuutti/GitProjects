// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;

namespace RestFoundation.Odata
{
	/// <summary>
	/// Defines the public interface for a model filter.
	/// </summary>
	/// <typeparam name="T">The <see cref="Type"/> of item to filter.</typeparam>
	public interface IModelFilter<in T>
	{
		/// <summary>
		/// Filters the passed collection with the defined filter.
		/// </summary>
		/// <param name="source">The source items to filter.</param>
		/// <returns>A filtered enumeration and projected of the source items.</returns>
		IEnumerable<object> Filter(IEnumerable<T> source);
	}
}