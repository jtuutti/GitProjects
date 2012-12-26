// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace RestFoundation.Odata.Parser
{
    [ExcludeFromCodeCoverage]
    internal class FunctionTokenSet : TokenSet
    {
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", Operation, Left, Right);
        }
    }
}