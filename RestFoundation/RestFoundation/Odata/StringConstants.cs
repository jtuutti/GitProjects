// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;

namespace RestFoundation.Odata
{
    [ExcludeFromCodeCoverage]
    internal static class StringConstants
    {
        internal const string OrderByParameter = "$orderby";
        internal const string SelectParameter = "$select";
        internal const string FilterParameter = "$filter";
        internal const string SkipParameter = "$skip";
        internal const string TopParameter = "$top";
    }
}