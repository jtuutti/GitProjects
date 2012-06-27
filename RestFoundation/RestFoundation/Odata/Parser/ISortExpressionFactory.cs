// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;

namespace RestFoundation.Odata.Parser
{
	/// <summary>
	/// Defines the public interface for a SortExpressionFactory.
	/// </summary>
	public interface ISortExpressionFactory
	{
		/// <summary>
		/// Creates an enumeration of sort descriptions from its string representation.
		/// </summary>
		/// <param name="filter">The string representation of the sort descriptions.</param>
		/// <typeparam name="T">The <see cref="Type"/> of item to sort.</typeparam>
		/// <returns>An <see cref="IEnumerable{T}"/> if the passed sort descriptions are valid, otherwise null.</returns>
		IEnumerable<SortDescription<T>> Create<T>(string filter);

		/// <summary>
		/// Creates an enumeration of sort descriptions from its string representation.
		/// </summary>
		/// <param name="filter">The string representation of the sort descriptions.</param>
		/// <param name="formatProvider">The <see cref="IFormatProvider"/> to use when reading the sort descriptions.</param>
		/// <typeparam name="T">The <see cref="Type"/> of item to sort.</typeparam>
		/// <returns>An <see cref="IEnumerable{T}"/> if the passed sort descriptions are valid, otherwise null.</returns>
		IEnumerable<SortDescription<T>> Create<T>(string filter, IFormatProvider formatProvider);
	}
}