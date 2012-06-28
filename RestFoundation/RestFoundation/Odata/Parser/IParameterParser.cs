// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System.Collections.Specialized;

namespace RestFoundation.Odata.Parser
{
	internal interface IParameterParser<in T>
	{
		IModelFilter<T> Parse(NameValueCollection queryParameters);
	}
}