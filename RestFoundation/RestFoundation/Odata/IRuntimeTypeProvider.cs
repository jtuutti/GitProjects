// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace RestFoundation.Odata
{
	/// <summary>
	/// Provides a type matching the provided members.
	/// </summary>
	public interface IRuntimeTypeProvider
	{
		/// <summary>
		/// Gets the <see cref="Type"/> matching the provided members.
		/// </summary>
		/// <param name="sourceType">The <see cref="Type"/> to generate the runtime type from.</param>
		/// <param name="properties">The <see cref="MemberInfo"/> to use to generate properties.</param>
		/// <returns>A <see cref="Type"/> mathing the provided properties.</returns>
		Type Get(Type sourceType, IEnumerable<MemberInfo> properties);
	}
}