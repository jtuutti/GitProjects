// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System;
using System.Reflection;

namespace RestFoundation.Odata.Parser
{
	/// <summary>
	/// Defines the public interface for a resolver of <see cref="MemberInfo"/> name.
	/// </summary>
	public interface IMemberNameResolver
	{
		/// <summary>
		/// Returns the resolved name for the <see cref="MemberInfo"/>.
		/// </summary>
		/// <param name="member">The <see cref="MemberInfo"/> to resolve the name of.</param>
		/// <returns>The resolved name.</returns>
		string ResolveName(MemberInfo member);
	}

	internal abstract class MemberNameResolverContracts : IMemberNameResolver
	{
		public string ResolveName(MemberInfo member)
		{
		    if (member == null) throw new ArgumentNullException("member");

		    throw new NotImplementedException();
		}
	}
}