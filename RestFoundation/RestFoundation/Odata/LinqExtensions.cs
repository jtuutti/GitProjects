// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.
// Based on code from http://stackoverflow.com/questions/606104/how-to-create-linq-expression-tree-with-anonymous-type-in-it

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RestFoundation.Odata
{
	internal static class LinqExtensions
	{
		public static bool IsAnonymousType(this Type type)
		{
		    if (type == null) throw new ArgumentNullException("type");

		    return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
				&& type.IsGenericType
				&& type.Name.Contains("AnonymousType") && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
				&& (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
		}
	}
}